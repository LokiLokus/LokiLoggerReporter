using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lokiloggerreporter.Middleware;

namespace lokiloggerreporter.Models
{
    public class Source
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SourceId { get; set; }
        [Required(ErrorMessage = "A Source Name must be defined")]
        public string Name { get; set; }
        public string Version { get; set; }
        public string Tag { get; set; }
        [NotMapped]
        public string LogUrl => UrlObtainer.AppBaseUrl + "/api/Logging/Log/" + SourceId;
        public string Description { get; set; }
        public string Secret { get; set; }
    }
}