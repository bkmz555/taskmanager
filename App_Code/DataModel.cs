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

public partial class DEFECT
{
    public decimal ProjectID { get; set; }
    public decimal idRecord { get; set; }
    public Nullable<System.DateTime> dateCreate { get; set; }
    public Nullable<decimal> idCreateBy { get; set; }
    public Nullable<System.DateTime> dateModify { get; set; }
    public Nullable<decimal> idModifyBy { get; set; }
    public Nullable<decimal> DefectNum { get; set; }
    public string Summary { get; set; }
    public Nullable<decimal> Status { get; set; }
    public Nullable<decimal> InitStatus { get; set; }
    public Nullable<decimal> idType { get; set; }
    public Nullable<decimal> idProduct { get; set; }
    public string Reference { get; set; }
    public Nullable<decimal> idEnterBy { get; set; }
    public Nullable<decimal> idDisposit { get; set; }
    public Nullable<decimal> idPriority { get; set; }
    public Nullable<decimal> idCompon { get; set; }
    public Nullable<decimal> idSeverity { get; set; }
    public Nullable<System.DateTime> dateEnter { get; set; }
    public Nullable<decimal> AddLocat { get; set; }
    public string Workaround { get; set; }
    public Nullable<decimal> idTicket { get; set; }
    public string Notify { get; set; }
    public Nullable<int> iOrder { get; set; }
    public string sModifier { get; set; }
    public Nullable<float> Estim { get; set; }
    public Nullable<float> Spent { get; set; }
    public string Usr { get; set; }
    public Nullable<System.DateTime> IOrderDate { get; set; }
    public Nullable<int> idUsr { get; set; }
    public string branch { get; set; }
    public Nullable<int> iBuildPriority { get; set; }
    public string branchBST { get; set; }
    public Nullable<int> idEstim { get; set; }
    public Nullable<System.DateTime> dateTimer { get; set; }
    public Nullable<int> attachs { get; set; }
    public string Version { get; set; }
    public Nullable<System.DateTime> EDD { get; set; }
}

public partial class DEFECTEVT
{
    public decimal ProjectID { get; set; }
    public decimal idRecord { get; set; }
    public Nullable<decimal> EvtDefID { get; set; }
    public Nullable<decimal> OrderNum { get; set; }
    public decimal ParentID { get; set; }
    public Nullable<decimal> EvtMUParnt { get; set; }
    public Nullable<decimal> idUser { get; set; }
    public Nullable<System.DateTime> dateEvent { get; set; }
    public string Notes { get; set; }
    public Nullable<decimal> TimeSpent { get; set; }
    public Nullable<decimal> RsltState { get; set; }
    public string RelVersion { get; set; }
    public string AsgndUsers { get; set; }
    public Nullable<decimal> GenByType { get; set; }
    public Nullable<decimal> CreatorID { get; set; }
    public Nullable<decimal> DefAsgEff { get; set; }
    public Nullable<decimal> OvrWF { get; set; }
    public Nullable<decimal> OvrWFUsrID { get; set; }
}

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
