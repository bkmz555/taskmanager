﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

public partial class DefectTracker
{
    public int idRecord { get; set; }
    public string Name { get; set; }
    public int idOwner { get; set; }
    public Nullable<int> idFilter { get; set; }
    public Nullable<int> idClient { get; set; }
    public Nullable<System.DateTime> dateCreated { get; set; }
    public string COLORDEF { get; set; }
}

public partial class Machine
{
    public string PCNAME { get; set; }
    public string IP { get; set; }
    public string MAC { get; set; }
    public string DETAILS { get; set; }
    public string CATEGORY { get; set; }
}
