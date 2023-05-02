using System;
using System.Collections.Generic;

namespace FaceDetectionAttendance.MVVM.Model;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string? Passwords { get; set; }

    public int Roles { get; set; }

    public string? Gmail { get; set; }

    public string? Images { get; set; }

    public string? Fid { get; set; }

    public virtual Faculty? FidNavigation { get; set; }
}
