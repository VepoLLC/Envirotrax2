

namespace Envirotrax.Common.Data.Models
{
    public interface IAuditableModel<TUser> : ICreateAuditableModel<TUser>, IUpdateAuditableModel<TUser>, IDeleteAutitableModel<TUser>
        where TUser : IAspNetUserBase
    {

    }

    public interface ICreateAuditableModel<TUser>
        where TUser : IAspNetUserBase
    {
        int? CreatedById { get; set; }
        TUser? CreatedBy { get; set; }
        DateTime CreatedTime { get; set; }
    }

    public interface IUpdateAuditableModel<TUser>
        where TUser : IAspNetUserBase
    {
        int? UpdatedById { get; set; }
        TUser? UpdatedBy { get; set; }
        DateTime? UpdatedTime { get; set; }
    }

    public interface IDeleteAutitableModel<TUser>
        where TUser : IAspNetUserBase
    {
        int? DeletedById { get; set; }
        TUser? DeletedBy { get; set; }
        DateTime? DeletedTime { get; set; }
    }
}