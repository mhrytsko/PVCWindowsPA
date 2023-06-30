using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    /// <summary>
    /// Budget -> N:N <- Window
    /// </summary>
    public class BudgetWindow : ModelBase
    {
        [Required, ForeignKey("Budget")]
        public Guid? BudgetId { get; set; }

        [Required, ForeignKey("Window")]
        public Guid? WindowId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction), JsonIgnore]
        public virtual Budget? Budget { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction), JsonIgnore]
        public virtual Window? Window { get; set; }
    }

    /// <summary>
    /// Os cores compativeis com os perfis de janelas.
    /// WindowColors -> N:N <- WindowProfile
    /// </summary>
    public class WindowProfileColor : ModelBase
    {
        [Required, ForeignKey("Color")]
        public Guid? ColorId { get; set; }

        [Required, ForeignKey("Profile")]
        public Guid? ProfileId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowColor? Color { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowProfile? Profile { get; set; }
    }
}