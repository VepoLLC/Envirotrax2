
using Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;
using Envirotrax.Common;

namespace Envirotrax.App.Server.Data.SeedData;

public static class FeatureSeedData
{
    public static IReadOnlyList<Feature> Features =>
    [
        new()
        {
            Id = FeatureType.ManagePermissions,
            Name = "Manage Permissions",
            Description = "Allows to manage roles and permissions. Allows assigning roles to users."
        },
        new()
        {
            Id = FeatureType.ManageProfessionalLicenses,
            Name = "Manage Professional Licenses",
            Description = "Aloows managing and verifying licenses of registred professionals."
        },
        new()
        {
            Id = FeatureType.ManageProfessionalInsurances,
            Name = "Manage Professional Insurances",
            Description = "Aloows managing and verifying insurances of registred professionals."
        },
        new()
        {
            Id = FeatureType.ManageProfessionalRegistrationFees,
            Name = "Manage Professional Fees",
            Description = "Allows managing fees for permorming various tests and inspections by registered professionals for each water supplier."
        },
        new()
        {
            Id = FeatureType.ManageGisAreas,
            Name = "Manage GIS Areas",
            Description = "Allow making changes to GIS areas. This includes creating, editing, deleting, and assiging areas to sites."
        }
    ];
}