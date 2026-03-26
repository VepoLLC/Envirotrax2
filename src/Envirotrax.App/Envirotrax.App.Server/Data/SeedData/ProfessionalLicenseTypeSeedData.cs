
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.States;

namespace Envirotrax.App.Server.Data.SeedData;


public static class ProfessionalLicenseTypeSeedData
{
    public static IEnumerable<ProfessionalLicenseType> GetTypes(IDictionary<string, State> states)
    {
        var types = new List<ProfessionalLicenseType>();

        // Kansas
        var kansas = states["KS"];
        types.Add(new()
        {
            Name = "ASSE - Tester",
            Description = "ASSE License",
            ProfessionalType = ProfessionalType.Bpat,
            StateId = kansas.Id
        });
        types.Add(new()
        {
            Name = "ASSE - Fire BP Tester",
            Description = "ASSE License",
            ProfessionalType = ProfessionalType.Bpat,
            StateId = kansas.Id
        });

        // Texas
        var texas = states["TX"];
        types.Add(new()
        {
            Name = "TCEQ - BPAT License",
            Description = "TCEQ - BPAT License",
            ProfessionalType = ProfessionalType.Bpat,
            StateId = texas.Id
        });

        types.Add(new()
        {
            Name = "TX Fire Marshal Office - SCR",
            Description = "TCEQ - BPAT License",
            ProfessionalType = ProfessionalType.Bpat,
            StateId = texas.Id
        });
        types.Add(new()
        {
            Name = "TCEQ - CSI License",
            Description = "Texas CSI/Plumbing Inspector/WSPS",
            ProfessionalType = ProfessionalType.CsiInspector,
            StateId = texas.Id
        });
        types.Add(new()
        {
            Name = "TSBPE - Plumbing Inspector",
            Description = "Texas CSI/Plumbing Inspector/WSPS",
            ProfessionalType = ProfessionalType.CsiInspector,
            StateId = texas.Id
        });
        types.Add(new()
        {
            Name = "TSBPE - Journeyman WSPS",
            Description = "Texas CSI/Plumbing Inspector/WSPS",
            ProfessionalType = ProfessionalType.CsiInspector,
            StateId = texas.Id
        });
        types.Add(new()
        {
            Name = "TSBPE - Master WSPS",
            Description = "Texas CSI/Plumbing Inspector/WSPS",
            ProfessionalType = ProfessionalType.CsiInspector,
            StateId = texas.Id
        });
        types.Add(new()
        {
            Name = "TCEQ - Registration Number",
            Description = "TCEQ - Registration Number",
            ProfessionalType = ProfessionalType.FogTransporter,
            StateId = texas.Id
        });

        // Washington
        var washington = states["WA"];
        types.Add(new()
        {
            Name = "WCS - BAT License",
            Description = "WCS - BAT License",
            ProfessionalType = ProfessionalType.Bpat,
            StateId = washington.Id
        });
        types.Add(new()
        {
            Name = "WCS - CSI License",
            Description = "WCS - CSI License",
            ProfessionalType = ProfessionalType.CsiInspector,
            StateId = washington.Id
        });
        types.Add(new()
        {
            Name = "WCS - Registration Number",
            Description = "WCS - Registration Number",
            ProfessionalType = ProfessionalType.FogTransporter,
            StateId = washington.Id
        });

        return types;
    }
}