using Envirotrax.Common.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Data.Models.States
{
    public class State
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        [StringLength(10)]
        public string Code { get; set; } = null!;
    }
}
