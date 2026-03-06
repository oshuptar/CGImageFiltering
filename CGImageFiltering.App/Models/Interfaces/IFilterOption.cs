using System;
using GCImageFiltering.Core.Filters.Interfaces;

namespace CGImageFiltering.App.Models.Interfaces;

public interface IFilterOption
{
    public string Name { get; set; }
    public Func<IFilter> FilterFactory { get; set; }
}