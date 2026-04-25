using System.ComponentModel.DataAnnotations.Schema;

namespace Hourglass.Models
{
    public class Time
    {

        public Time()
        {

        }

        [Column("time_id")]
        public int Id { get; set; }

        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public Project Project { get; set; }

        public User User { get; set; }

        [Column("started_at")]
        public DateTime StartedAt { get; set; }

        [Column("ended_at")]
        public DateTime EndedAt { get; set; }
    }
}
