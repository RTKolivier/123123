using System;
using System.Collections.Generic;

namespace diplomdemo.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? UserRole { get; set; }

    public string? UserName { get; set; }

    public string? UserPhone { get; set; }

    public string? UserLogin { get; set; }

    public string? UserPassword { get; set; }

    public virtual Role? UserRoleNavigation { get; set; }
}
