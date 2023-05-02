using System;
using System.Collections.Generic;

namespace FaceDetectionAttendance.MVVM.Model;

public partial class Faculty
{
    public string IdFaculty { get; set; } = null!;

    public string? NameFaculty { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<WorkerList> WorkerLists { get; set; } = new List<WorkerList>();
}
