using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GraceProject.Models;

public class Report
{
    public string SearchType { get; set; }
    public string Value { get; set; }
    public string ReportType { get; set; }
    public string Course { get; set; }

    [DataType(DataType.Date)]
    public DateTime FromDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime ToDate { get; set; }

    public string ChartScale { get; set; }
}


