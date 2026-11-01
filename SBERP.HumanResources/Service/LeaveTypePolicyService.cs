using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Data.Common;
using System.Net;

namespace SBERP.HumanResources.Service
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public LeaveTypeService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<LeaveTypeResponse>?> GetAllLeaveTypesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<LeaveTypeResponse>(_ctx, ConstantSupplier.SP_GET_ALL_LEAVE_TYPES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllLeaveTypesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetLeaveTypesLookupAsync()
        {
            try
            {
                var items = await _ctx.LeaveTypes!.Where(t => t.IsActive == true)
                    .OrderBy(t => t.Name)
                    .Select(t => new LeaveTypeResponse
                    {
                        Id = t.Id, LeaveTypeCode = t.LeaveTypeCode, Name = t.Name, IsPaid = t.IsPaid,
                        AllowHalfDay = t.AllowHalfDay, RequiresAttachment = t.RequiresAttachment,
                        IsCarryForward = t.IsCarryForward, ColorHex = t.ColorHex, IsActive = t.IsActive
                    }).ToListAsync();
                return Utilities.Ok(ConstantSupplier.LEAVE_TYPE_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetLeaveTypesLookupAsync)); }
        }

        public async Task<DataResponse> CreateUpdateLeaveTypeAsync(LeaveTypeRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                bool dup = await _ctx.LeaveTypes!.AnyAsync(t => t.LeaveTypeCode == request.LeaveTypeCode && (!isUpdate || t.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Leave type code already exists.");

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid leave type id");
                    var t = await _ctx.LeaveTypes!.FirstOrDefaultAsync(x => x.Id == id);
                    if (t == null) return Utilities.Warn("Leave type not found", code: (int)HttpStatusCode.NotFound);
                    t.LeaveTypeCode = request.LeaveTypeCode; t.Name = request.Name; t.IsPaid = request.IsPaid;
                    t.AllowHalfDay = request.AllowHalfDay; t.RequiresAttachment = request.RequiresAttachment;
                    t.IsCarryForward = request.IsCarryForward; t.MaxCarryForward = request.MaxCarryForward;
                    t.AccrualPerMonth = request.AccrualPerMonth; t.ColorHex = request.ColorHex; t.Description = request.Description;
                    t.UpdatedBy = request.CreateUpdateBy; t.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    await _ctx.LeaveTypes!.AddAsync(new LeaveType
                    {
                        Id = Guid.NewGuid(), LeaveTypeCode = request.LeaveTypeCode, Name = request.Name,
                        IsPaid = request.IsPaid, AllowHalfDay = request.AllowHalfDay, RequiresAttachment = request.RequiresAttachment,
                        IsCarryForward = request.IsCarryForward, MaxCarryForward = request.MaxCarryForward,
                        AccrualPerMonth = request.AccrualPerMonth, ColorHex = request.ColorHex, Description = request.Description,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateLeaveTypeAsync)); }
        }

        public async Task<DataResponse> DeleteLeaveTypeAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid leave type id");
                var t = await _ctx.LeaveTypes!.FirstOrDefaultAsync(x => x.Id == g);
                if (t == null) return Utilities.Warn("Leave type not found", code: (int)HttpStatusCode.NotFound);
                bool inUse = await _ctx.LeaveApplications!.AnyAsync(a => a.LeaveTypeId == g)
                          || await _ctx.LeavePolicies!.AnyAsync(p => p.LeaveTypeId == g && p.IsActive == true);
                if (inUse) return Utilities.Warn(ConstantSupplier.LEAVE_TYPE_IN_USE);
                t.IsActive = false; t.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteLeaveTypeAsync)); }
        }
    }

    public class LeavePolicyService : ILeavePolicyService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public LeavePolicyService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<LeavePolicyResponse>?> GetAllLeavePoliciesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<LeavePolicyResponse>(_ctx, ConstantSupplier.SP_GET_ALL_LEAVE_POLICIES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllLeavePoliciesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateLeavePolicyAsync(LeavePolicyRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.LeaveTypeId, out var typeId)) return Utilities.Warn("Invalid leave type id");
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                bool dup = await _ctx.LeavePolicies!.AnyAsync(p => p.PolicyCode == request.PolicyCode && (!isUpdate || p.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Policy code already exists.");

                Guid? dept = Guid.TryParse(request.DepartmentId, out var d) ? d : null;

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid policy id");
                    var p = await _ctx.LeavePolicies!.FirstOrDefaultAsync(x => x.Id == id);
                    if (p == null) return Utilities.Warn("Policy not found", code: (int)HttpStatusCode.NotFound);
                    p.PolicyCode = request.PolicyCode; p.Name = request.Name; p.LeaveTypeId = typeId;
                    p.DepartmentId = dept; p.Grade = request.Grade; p.AnnualEntitlement = request.AnnualEntitlement;
                    p.MinServiceMonths = request.MinServiceMonths; p.MaxConsecutiveDays = request.MaxConsecutiveDays;
                    p.EffectiveYear = request.EffectiveYear; p.UpdatedBy = request.CreateUpdateBy; p.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    await _ctx.LeavePolicies!.AddAsync(new LeavePolicy
                    {
                        Id = Guid.NewGuid(), PolicyCode = request.PolicyCode, Name = request.Name, LeaveTypeId = typeId,
                        DepartmentId = dept, Grade = request.Grade, AnnualEntitlement = request.AnnualEntitlement,
                        MinServiceMonths = request.MinServiceMonths, MaxConsecutiveDays = request.MaxConsecutiveDays,
                        EffectiveYear = request.EffectiveYear, CreatedBy = request.CreateUpdateBy,
                        CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateLeavePolicyAsync)); }
        }

        public async Task<DataResponse> DeleteLeavePolicyAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid policy id");
                var p = await _ctx.LeavePolicies!.FirstOrDefaultAsync(x => x.Id == g);
                if (p == null) return Utilities.Warn("Policy not found", code: (int)HttpStatusCode.NotFound);
                p.IsActive = false; p.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteLeavePolicyAsync)); }
        }

        public async Task<DataResponse> GrantEntitlementsAsync(GrantEntitlementsRequest request)
        {
            try
            {
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_GRANT_LEAVE_ENTITLEMENTS,
                    ("@Year", request.Year), ("@CreatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GrantEntitlementsAsync)); }
        }
    }
}
