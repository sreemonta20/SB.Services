using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Net;

namespace SBERP.HumanResources.Service
{
    /// <summary>
    /// Backbone of HR. EF Core handles the writes inside a single transaction
    /// so a partial Employee never leaks (Employee + Addresses + Education +
    /// ... are saved or rolled back together). Reads use SPs because the
    /// nested JSON shape is cheaper to build server-side.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public EmployeeService(HumanResourcesDBContext ctx, IHRLogService log)
        {
            _ctx = ctx; _log = log;
        }

        // -----------------------------------------------------------------
        // Initial data (dropdowns + next employee code)
        // -----------------------------------------------------------------
        public async Task<DataResponse> GetEmployeeInitialDataAsync()
        {
            try
            {
                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_GET_EMPLOYEE_INITIAL_DATA;

                var raw = (string?)await cmd.ExecuteScalarAsync();

                // Build static enum dropdowns in C# — no point storing in DB.
                //var resp = new EmployeeInitialDataResponse
                //{
                //    Genders = new List<LookupItem>
                //    {
                //        new() { Id = "1", Name = "Male"   },
                //        new() { Id = "2", Name = "Female" },
                //        new() { Id = "3", Name = "Other"  }
                //    },
                //    MaritalStatuses = new List<LookupItem>
                //    {
                //        new() { Id = "1", Name = "Single"   },
                //        new() { Id = "2", Name = "Married"  },
                //        new() { Id = "3", Name = "Divorced" },
                //        new() { Id = "4", Name = "Widowed"  },
                //        new() { Id = "5", Name = "Separated"}
                //    },
                //    BloodGroups = new List<LookupItem>
                //    {
                //        new() { Id = "1", Name = "A+"  }, new() { Id = "2", Name = "A-"  },
                //        new() { Id = "3", Name = "B+"  }, new() { Id = "4", Name = "B-"  },
                //        new() { Id = "5", Name = "AB+" }, new() { Id = "6", Name = "AB-" },
                //        new() { Id = "7", Name = "O+"  }, new() { Id = "8", Name = "O-"  }
                //    },
                //    EmploymentTypes = new List<LookupItem>
                //    {
                //        new() { Id = "1", Name = "Full Time"  },
                //        new() { Id = "2", Name = "Part Time"  },
                //        new() { Id = "3", Name = "Contract"   },
                //        new() { Id = "4", Name = "Intern"     },
                //        new() { Id = "5", Name = "Consultant" },
                //        new() { Id = "6", Name = "Probation"  }
                //    },
                //    EmploymentStatuses = new List<LookupItem>
                //    {
                //        new() { Id = "1", Name = "Active"     },
                //        new() { Id = "2", Name = "On Leave"   },
                //        new() { Id = "3", Name = "Suspended"  },
                //        new() { Id = "4", Name = "Resigned"   },
                //        new() { Id = "5", Name = "Terminated" },
                //        new() { Id = "6", Name = "Retired"    }
                //    }
                //};

                var resp = new EmployeeInitialDataResponse();

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var jo = JObject.Parse(raw);
                    resp.Departments       = ParseLookup(jo["departments"]?.ToString());
                    resp.Designations      = ParseLookup(jo["designations"]?.ToString());
                    resp.ReportingManagers = ParseLookup(jo["reportingManagers"]?.ToString());
                    resp.Genders = ParseLookup(jo["genders"]?.ToString());
                    resp.MaritalStatuses = ParseLookup(jo["maritalStatuses"]?.ToString());
                    resp.BloodGroups = ParseLookup(jo["bloodGroups"]?.ToString());
                    resp.EmploymentTypes = ParseLookup(jo["employmentTypes"]?.ToString());
                    resp.EmploymentStatuses = ParseLookup(jo["employmentStatuses"]?.ToString());
                    resp.NextEmployeeCode  = jo["nextEmployeeCode"]?.ToString();
                }

                return Utilities.Ok("Employee initial data fetched.", resp);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeInitialDataAsync)); }
        }

        // -----------------------------------------------------------------
        // Paging
        // -----------------------------------------------------------------
        public async Task<PagingResult<EmployeeListResponse>?> GetAllEmployeesPagingWithSearchAsync(
            PagingSearchFilter filter)
        {
            try
            {
                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    "EXEC dbo." + ConstantSupplier.SP_GET_ALL_EMPLOYEES_PAGING +
                    " @SearchTerm=@p0, @SortColumnName=@p1, " +
                    "@SortColumnDirection=@p2, @PageNumber=@p3, @PageSize=@p4";

                AddParam(cmd, "@p0", filter.SearchTerm);
                AddParam(cmd, "@p1", filter.SortColumnName);
                AddParam(cmd, "@p2", filter.SortColumnDirection);
                AddParam(cmd, "@p3", filter.PageNumber);
                AddParam(cmd, "@p4", filter.PageSize);

                var raw = (string?)await cmd.ExecuteScalarAsync();
                if (string.IsNullOrWhiteSpace(raw)) return null;

                var jo = JObject.Parse(raw);
                var items = jo["Items"]?.ToString();

                return new PagingResult<EmployeeListResponse>
                {
                    RowCount    = jo.Value<int>("RowCount"),
                    CurrentPage = jo.Value<int>("CurrentPage"),
                    PageSize    = jo.Value<int>("PageSize"),
                    PageCount   = jo.Value<int>("PageCount"),
                    Items       = string.IsNullOrWhiteSpace(items)
                                  ? new List<EmployeeListResponse>()
                                  : JsonConvert.DeserializeObject<List<EmployeeListResponse>>(items)
                };
            }
            catch (Exception ex)
            {
                Utilities.Exception(ex, _log, nameof(GetAllEmployeesPagingWithSearchAsync));
                return null;
            }
        }

        // -----------------------------------------------------------------
        // Get by id — returns the full nested record (JSON from SP)
        // -----------------------------------------------------------------
        public async Task<DataResponse> GetEmployeeByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g))
                    return Utilities.Warn("Invalid employee id");

                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_GET_EMPLOYEE_BY_ID + " @Id=@p0";
                AddParam(cmd, "@p0", g);

                // SP returns JSON in a single-column row
                using var reader = await cmd.ExecuteReaderAsync();
                var sb = new System.Text.StringBuilder();
                while (await reader.ReadAsync())
                    sb.Append(reader.GetValue(0)?.ToString());

                var json = sb.ToString();
                if (string.IsNullOrWhiteSpace(json))
                    return Utilities.Warn(ConstantSupplier.EMPLOYEE_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                // Inner FOR JSON columns come back as escaped strings — parse them.
                var jo = JObject.Parse(json);
                var detail = jo.ToObject<EmployeeDetailResponse>()!;

                detail.Addresses         = ParseJsonArray<EmployeeAddressDto>(jo["Addresses"]?.ToString());
                detail.Educations        = ParseJsonArray<EmployeeEducationDto>(jo["Educations"]?.ToString());
                detail.Experiences       = ParseJsonArray<EmployeeExperienceDto>(jo["Experiences"]?.ToString());
                detail.Skills            = ParseJsonArray<EmployeeSkillDto>(jo["Skills"]?.ToString());
                detail.Trainings         = ParseJsonArray<EmployeeTrainingDto>(jo["Trainings"]?.ToString());
                detail.Certifications    = ParseJsonArray<EmployeeCertificationDto>(jo["Certifications"]?.ToString());
                detail.Documents         = ParseJsonArray<EmployeeDocumentDto>(jo["Documents"]?.ToString());
                detail.EmergencyContacts = ParseJsonArray<EmployeeEmergencyContactDto>(jo["EmergencyContacts"]?.ToString());

                var bankRaw = jo["BankInfo"]?.ToString();
                if (!string.IsNullOrWhiteSpace(bankRaw))
                    detail.BankInfo = JsonConvert.DeserializeObject<EmployeeBankDto>(bankRaw);

                return Utilities.Ok(ConstantSupplier.EMPLOYEE_FETCH_SUCCESS, detail);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeByIdAsync)); }
        }

        // -----------------------------------------------------------------
        // Create / Update — single transaction, whole graph
        // -----------------------------------------------------------------
        public async Task<DataResponse> CreateUpdateEmployeeAsync(EmployeeRequest request)
        {
            if (request == null) return Utilities.Warn(ConstantSupplier.REQ_OR_DATA_NULL);

            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName,
                    ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);

                Employee emp;

                if (isSave)
                {
                    // Reject duplicate EmployeeCode early.
                    bool dup = await _ctx.Employees!
                        .AnyAsync(e => e.EmployeeCode == request.EmployeeCode);
                    if (dup) return Utilities.Warn(ConstantSupplier.EMPLOYEE_EXIST_DUPL_CODE);

                    emp = new Employee
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = request.EmployeeCode,
                        CreatedBy    = request.CreateUpdateBy,
                        CreatedDate  = DateTime.UtcNow
                    };
                    MapEmployeeFields(emp, request);
                    await _ctx.Employees!.AddAsync(emp);
                    await _ctx.SaveChangesAsync();   // Get emp.Id before children
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id))
                        return Utilities.Warn("Invalid employee id for update");

                    var existing = await _ctx.Employees!
                        .Include(e => e.Addresses)
                        .Include(e => e.Educations)
                        .Include(e => e.Experiences)
                        .Include(e => e.Skills)
                        .Include(e => e.Trainings)
                        .Include(e => e.Certifications)
                        .Include(e => e.Documents)
                        .Include(e => e.EmergencyContacts)
                        .Include(e => e.BankInfo)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existing == null)
                        return Utilities.Warn(ConstantSupplier.EMPLOYEE_NOT_FOUND,
                                              code: (int)HttpStatusCode.NotFound);

                    MapEmployeeFields(existing, request);
                    existing.UpdatedBy   = request.CreateUpdateBy;
                    existing.UpdatedDate = DateTime.UtcNow;

                    // Replace children with what came in. Pragmatic and matches
                    // what the form actually does — sends the whole shape every
                    // save. For diff-based updates change strategy here.
                    if (existing.Addresses?.Count > 0)
                        _ctx.EmployeeAddresses!.RemoveRange(existing.Addresses);
                    if (existing.Educations?.Count > 0)
                        _ctx.EmployeeEducations!.RemoveRange(existing.Educations);
                    if (existing.Experiences?.Count > 0)
                        _ctx.EmployeeExperiences!.RemoveRange(existing.Experiences);
                    if (existing.Skills?.Count > 0)
                        _ctx.EmployeeSkills!.RemoveRange(existing.Skills);
                    if (existing.Trainings?.Count > 0)
                        _ctx.EmployeeTrainings!.RemoveRange(existing.Trainings);
                    if (existing.Certifications?.Count > 0)
                        _ctx.EmployeeCertifications!.RemoveRange(existing.Certifications);
                    if (existing.Documents?.Count > 0)
                        _ctx.EmployeeDocuments!.RemoveRange(existing.Documents);
                    if (existing.EmergencyContacts?.Count > 0)
                        _ctx.EmployeeEmergencyContacts!.RemoveRange(existing.EmergencyContacts);
                    if (existing.BankInfo != null)
                        _ctx.EmployeeBanks!.Remove(existing.BankInfo);

                    await _ctx.SaveChangesAsync();
                    emp = existing;
                }

                // Insert child collections fresh in both Save and Update paths
                await PersistChildrenAsync(emp.Id, request);

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Utilities.Ok(
                    isSave ? ConstantSupplier.EMPLOYEE_CREATE_SUCCESS
                           : ConstantSupplier.EMPLOYEE_UPDATE_SUCCESS,
                    emp.Id);
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueViolation(dbEx))
            {
                await tx.RollbackAsync();
                return Utilities.Warn(ConstantSupplier.EMPLOYEE_EXIST_DUPL_CODE);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateEmployeeAsync));
            }
        }

        // -----------------------------------------------------------------
        // Delete (soft by default)
        // -----------------------------------------------------------------
        public async Task<DataResponse> DeleteEmployeeAsync(string id, bool hardDelete = false)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(id, out var g))
                    return Utilities.Warn("Invalid employee id");

                var emp = await _ctx.Employees!.FirstOrDefaultAsync(e => e.Id == g);
                if (emp == null)
                    return Utilities.Warn(ConstantSupplier.EMPLOYEE_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                if (hardDelete)
                {
                    _ctx.Employees!.Remove(emp);   // cascade deletes children
                }
                else
                {
                    emp.IsActive = false;
                    emp.EmploymentStatus = (int)EnumEmploymentStatus.Resigned;
                    emp.DateOfLeaving ??= DateTime.UtcNow;
                }
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.EMPLOYEE_DELETE_SUCCESS, emp.Id);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(DeleteEmployeeAsync));
            }
        }

        // =================================================================
        // helpers
        // =================================================================
        private static void MapEmployeeFields(Employee e, EmployeeRequest r)
        {
            e.AppUserId             = Guid.TryParse(r.AppUserId, out var u) ? u : null;
            e.FirstName             = r.FirstName;
            e.MiddleName            = r.MiddleName;
            e.LastName              = r.LastName;
            e.FullName              = ComposeFullName(r.FirstName, r.MiddleName, r.LastName);
            e.OfficialEmail         = r.OfficialEmail;
            e.PersonalEmail         = r.PersonalEmail;
            e.DateOfBirth           = r.DateOfBirth;
            e.Gender                = r.Gender;
            e.MaritalStatus         = r.MaritalStatus;
            e.BloodGroup            = r.BloodGroup;
            e.Nationality           = r.Nationality;
            e.Religion              = r.Religion;
            e.NationalId            = r.NationalId;
            e.PassportNumber        = r.PassportNumber;
            e.PassportExpiryDate    = r.PassportExpiryDate;
            e.MobileNumber          = r.MobileNumber;
            e.AlternatePhoneNumber  = r.AlternatePhoneNumber;
            e.DepartmentId          = Guid.TryParse(r.DepartmentId,        out var d) ? d : null;
            e.DesignationId         = Guid.TryParse(r.DesignationId,       out var dg) ? dg : null;
            e.ReportingManagerId    = Guid.TryParse(r.ReportingManagerId,  out var m) ? m : null;
            e.DateOfJoining         = r.DateOfJoining;
            e.ProbationEndDate      = r.ProbationEndDate;
            e.ConfirmationDate      = r.ConfirmationDate;
            e.DateOfLeaving         = r.DateOfLeaving;
            e.EmploymentType        = r.EmploymentType;
            e.EmploymentStatus      = r.EmploymentStatus ?? (int)EnumEmploymentStatus.Active;
            e.WorkLocation          = r.WorkLocation;
            e.BasicSalary           = r.BasicSalary;
            e.SalaryCurrency        = r.SalaryCurrency;
            e.PhotoUrl              = r.PhotoUrl;
            e.SignatureUrl          = r.SignatureUrl;
            e.IsActive              = r.IsActive ?? true;
        }

        private static string ComposeFullName(string? f, string? m, string? l)
        {
            var parts = new[] { f, m, l }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s!.Trim());
            return string.Join(' ', parts);
        }

        private async Task PersistChildrenAsync(Guid empId, EmployeeRequest r)
        {
            if (r.Addresses != null && r.Addresses.Count > 0)
                await _ctx.EmployeeAddresses!.AddRangeAsync(r.Addresses.Select(a => new EmployeeAddress
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    AddressType = a.AddressType, Line1 = a.Line1, Line2 = a.Line2,
                    City = a.City, State = a.State, Country = a.Country, PostalCode = a.PostalCode,
                    IsPrimary = a.IsPrimary, IsActive = a.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Educations != null && r.Educations.Count > 0)
                await _ctx.EmployeeEducations!.AddRangeAsync(r.Educations.Select(x => new EmployeeEducation
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    Qualification = x.Qualification, Institution = x.Institution,
                    University = x.University, Specialization = x.Specialization,
                    StartYear = x.StartYear, EndYear = x.EndYear, Grade = x.Grade,
                    DocumentUrl = x.DocumentUrl, IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Experiences != null && r.Experiences.Count > 0)
                await _ctx.EmployeeExperiences!.AddRangeAsync(r.Experiences.Select(x => new EmployeeExperience
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    CompanyName = x.CompanyName, Designation = x.Designation,
                    StartDate = x.StartDate, EndDate = x.EndDate,
                    Responsibilities = x.Responsibilities, Location = x.Location,
                    IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Skills != null && r.Skills.Count > 0)
                await _ctx.EmployeeSkills!.AddRangeAsync(r.Skills.Select(x => new EmployeeSkill
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    SkillName = x.SkillName, ProficiencyLevel = x.ProficiencyLevel,
                    YearsOfExperience = x.YearsOfExperience,
                    IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Trainings != null && r.Trainings.Count > 0)
                await _ctx.EmployeeTrainings!.AddRangeAsync(r.Trainings.Select(x => new EmployeeTraining
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    TrainingName = x.TrainingName, Provider = x.Provider,
                    StartDate = x.StartDate, EndDate = x.EndDate,
                    Outcome = x.Outcome, CertificateUrl = x.CertificateUrl,
                    IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Certifications != null && r.Certifications.Count > 0)
                await _ctx.EmployeeCertifications!.AddRangeAsync(r.Certifications.Select(x => new EmployeeCertification
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    CertificationName = x.CertificationName, IssuingAuthority = x.IssuingAuthority,
                    CertificationNumber = x.CertificationNumber,
                    IssueDate = x.IssueDate, ExpiryDate = x.ExpiryDate,
                    CertificateUrl = x.CertificateUrl,
                    IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.Documents != null && r.Documents.Count > 0)
                await _ctx.EmployeeDocuments!.AddRangeAsync(r.Documents.Select(x => new EmployeeDocument
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    DocumentType = x.DocumentType, DocumentNumber = x.DocumentNumber,
                    FileUrl = x.FileUrl, IssueDate = x.IssueDate, ExpiryDate = x.ExpiryDate,
                    Remark = x.Remark, IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.EmergencyContacts != null && r.EmergencyContacts.Count > 0)
                await _ctx.EmployeeEmergencyContacts!.AddRangeAsync(r.EmergencyContacts.Select(x => new EmployeeEmergencyContact
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    Name = x.Name, Relationship = x.Relationship,
                    PrimaryPhone = x.PrimaryPhone, AlternatePhone = x.AlternatePhone,
                    Email = x.Email, Address = x.Address,
                    IsPrimary = x.IsPrimary, IsActive = x.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                }));

            if (r.BankInfo != null && !string.IsNullOrWhiteSpace(r.BankInfo.AccountNumber))
                await _ctx.EmployeeBanks!.AddAsync(new EmployeeBank
                {
                    Id = Guid.NewGuid(), EmployeeId = empId,
                    BankName = r.BankInfo.BankName, BranchName = r.BankInfo.BranchName,
                    AccountHolderName = r.BankInfo.AccountHolderName,
                    AccountNumber = r.BankInfo.AccountNumber,
                    IbanNumber = r.BankInfo.IbanNumber, SwiftCode = r.BankInfo.SwiftCode,
                    RoutingNumber = r.BankInfo.RoutingNumber, Currency = r.BankInfo.Currency,
                    IsActive = r.BankInfo.IsActive ?? true,
                    CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow
                });
        }

        private static List<LookupItem>? ParseLookup(string? json)
            => string.IsNullOrWhiteSpace(json)
               ? new List<LookupItem>()
               : JsonConvert.DeserializeObject<List<LookupItem>>(json);

        private static List<T>? ParseJsonArray<T>(string? json)
            => string.IsNullOrWhiteSpace(json)
               ? new List<T>()
               : JsonConvert.DeserializeObject<List<T>>(json);

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
