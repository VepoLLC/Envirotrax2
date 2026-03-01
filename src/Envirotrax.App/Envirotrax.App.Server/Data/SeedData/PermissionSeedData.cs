using Envirotrax.App.Server.Data.Models.Users;

namespace Envirotrax.App.Server.Data.SeedData;

public static class PermissionSeedData
{
    public static IReadOnlyList<Permission> Permissions =>
    [
        // General
        new()
        {
            Id = PermissionType.WaterSuppliers,
            Category = PermissionCategoryType.General,
            Name = "Water Suppliers",
            SortOrder = 1,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true
        },
        new()
        {
            Id = PermissionType.Settings,
            Category = PermissionCategoryType.General,
            Name = "Settings",
            SortOrder = 2,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.Roles,
            Category = PermissionCategoryType.General,
            Name = "Roles",
            SortOrder = 3,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true
        },
        new()
        {
            Id = PermissionType.AccountInformation,
            Category = PermissionCategoryType.General,
            Name = "Account Information",
            SortOrder = 4,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.Users,
            Category = PermissionCategoryType.General,
            Name = "Users",
            SortOrder = 5,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true
        },
        new()
        {
            Id = PermissionType.Notifications,
            Category = PermissionCategoryType.General,
            Name = "Notifications",
            SortOrder = 6,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.Sites,
            Category = PermissionCategoryType.General,
            Name = "Sites",
            SortOrder = 7,
            CanView = true,
            CanCreate = true,
            CanEdit = true,
            CanDelete = true
        },

        // CSI
        new()
        {
            Id = PermissionType.CsiInspections,
            Category = PermissionCategoryType.Csi,
            Name = "Inspections",
            SortOrder = 8,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.CsiInspectors,
            Category = PermissionCategoryType.Csi,
            Name = "Inspectors",
            SortOrder = 9,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.CsiReports,
            Category = PermissionCategoryType.Csi,
            Name = "Reports",
            SortOrder = 10,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },

        // Backflow
        new()
        {
            Id = PermissionType.BackflowTests,
            Category = PermissionCategoryType.Backflow,
            Name = "Backflow Tests",
            SortOrder = 11,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.BackflowTesters,
            Category = PermissionCategoryType.Backflow,
            Name = "BPAT Management",
            SortOrder = 12,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.BackflowOutOfService,
            Category = PermissionCategoryType.Backflow,
            Name = "Out of Service",
            SortOrder = 13,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.BackflowReports,
            Category = PermissionCategoryType.Backflow,
            Name = "Reports",
            SortOrder = 14,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },

        // FOG
        new()
        {
            Id = PermissionType.FogTripTickets,
            Category = PermissionCategoryType.Fog,
            Name = "Trip Tickets",
            SortOrder = 15,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.FogVehicles,
            Category = PermissionCategoryType.Fog,
            Name = "Vehicle Management",
            SortOrder = 16,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.FogTransporters,
            Category = PermissionCategoryType.Fog,
            Name = "Transporter Management",
            SortOrder = 17,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.FogInspections,
            Category = PermissionCategoryType.Fog,
            Name = "Inspections",
            SortOrder = 18,
            CanView = true,
            CanCreate = false,
            CanEdit = true,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.FogInspectors,
            Category = PermissionCategoryType.Fog,
            Name = "Inspector Management",
            SortOrder = 19,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
        new()
        {
            Id = PermissionType.FogReports,
            Category = PermissionCategoryType.Fog,
            Name = "Reports",
            SortOrder = 20,
            CanView = true,
            CanCreate = false,
            CanEdit = false,
            CanDelete = false
        },
    ];
}
