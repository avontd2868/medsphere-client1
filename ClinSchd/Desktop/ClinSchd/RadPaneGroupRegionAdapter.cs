using System;
using Microsoft.Practices.Composite.Regions;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.Composite.Presentation.Regions;
using Telerik.Windows.Controls;

namespace ClinSchd
{
	public class RadPaneGroupRegionAdapter : RegionAdapterBase<RadPaneGroup>
	{
		public RadPaneGroupRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		protected override void Adapt(IRegion region, RadPaneGroup regionTarget)
		{
			region.Views.CollectionChanged += (s, e) =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (var item in e.NewItems.OfType<RadPane>())
						{
							//regionTarget.Items.Add(item);
							regionTarget.Items.Insert (0, item);
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (var item in e.OldItems.OfType<RadPane>())
						{
							item.RemoveFromParent();
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						var oldItems = e.OldItems.OfType<RadPane>();
						var newItems = e.NewItems.OfType<RadPane>();
						var newItemsEnumerator = newItems.GetEnumerator();
						foreach (var oldItem in oldItems)
						{
#if SILVERLIGHT	// SDM
							var parent = oldItem.Parent as Telerik.Windows.Controls.ItemsControl;
#else
							var parent = oldItem.Parent as System.Windows.Controls.ItemsControl;
#endif
							if (parent != null && parent.Items.Contains(oldItem))
							{
								parent.Items[parent.Items.IndexOf(oldItem)] = newItemsEnumerator.Current;
								if (!newItemsEnumerator.MoveNext())
								{
									break;
								}
							}
							else
							{
								oldItem.RemoveFromParent();
								regionTarget.Items.Add(newItemsEnumerator.Current);
							}
						}
						break;
					case NotifyCollectionChangedAction.Reset:
						regionTarget
							.EnumeratePanes()
							.ToList()
							.ForEach(p => p.RemoveFromParent());
						break;
					default:
						break;
				}
			};

			foreach (var view in region.Views.OfType<RadPane>())
			{
				regionTarget.Items.Add(view);
			}
		}

		protected override IRegion CreateRegion()
		{
			return new AllActiveRegion();
		}
	}
}
