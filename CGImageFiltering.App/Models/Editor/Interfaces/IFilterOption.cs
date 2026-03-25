using System;
using GCImageFiltering.Core.Filters.Interfaces;

namespace CGImageFiltering.App.Models.Editor.Interfaces;

public interface IFilterOption
{
    public string Name { get; set; }

    public double? Parameter { get; set; }
    public string? ParameterName { get; }
    public double? MinParameterValue { get; }
    public double? MaxParameterValue { get;}
    public double? ParameterIncrement { get; }
    public virtual bool HasParameter => Parameter != null;
    public Func<double?, IFilter> FilterFactory { get; set; }
}