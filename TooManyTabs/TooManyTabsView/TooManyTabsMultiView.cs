using Get.Data.Properties;
using Get.Data.DataTemplates;
using Gtudios.UI.Controls.Tabs;
using Get.Data.Bindings;
using Get.Data.Helpers;
using Get.Data.Collections.Update;
using Get.Data.Bundles;
using Get.Data.Collections;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
partial class TooManyTabsMultiView<T> : TemplateControl<TabView<TooManyTabsItem<T>>>
{
    public IProperty<TooManyTabsMultiItem<T>> TooManyTabsMultiItemProperty { get; }
    TooManyTabsView<T> _Parent;
    public TooManyTabsMultiView(TooManyTabsView<T> Parent, TooManyTabsMultiItem<T> initial)
    {
        _Parent = Parent;
        TooManyTabsMultiItemProperty = Auto(initial);
    }
    readonly IUpdateCollection<TooManyTabsItem<T>> empty = new UpdateCollection<TooManyTabsItem<T>>();
    //readonly ReadOnlyProperty<int> NegativeOne = new(-1);
    protected override void Initialize(TabView<TooManyTabsItem<T>> rootElement)
    {
        rootElement.TargetCollectionProperty.ValueChanged += delegate
        {
            Debugger.Break();
        };
        void Set(TooManyTabsMultiItem<T> newItem)
        {
            rootElement.SelectionManagerBinding = OneWay(newItem.ChildrenSelectionManagerProperty);
            //rootElement.PreferAlwaysSelectItemProperty.Value = false;
            //rootElement.SelectedIndexProperty.Bind(NegativeOne, ReadOnlyBindingModes.OneWay);
            //rootElement.ItemsSourceProperty.Value = newItem.Tabs;
            //rootElement.SelectedIndexProperty.Bind(newItem.SelectedIndexProperty, BindingModes.TwoWay);
            //rootElement.PreferAlwaysSelectItemProperty.Value = true;
        }
        TooManyTabsMultiItemProperty.ApplyAndRegisterForNewValue((_, x) => Set(x));
        rootElement.ConnectionContextBinding = OneWay(_Parent.ConnectionContextProperty);
        rootElement.ItemTemplate = new DataTemplate<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>>(
            tmti =>
            new TabItem<TooManyTabsItem<T>>
            {
                ContentBundle = new ContentBundle<TooManyTabsItem<T>, UIElement?>(default)
                .WithCustomCode(x =>
                    x.ContentProperty.Bind(tmti, ReadOnlyBindingModes.OneWay)
                )
                .WithCustomCode(x =>
                    x.ContentTemplate = 
                    DataTemplates.TextBlock<TooManyTabsItem<T>>(x => x.SelectPath(x => x.NameProperty))
                    .DataTemplateHelper().As<UIElement>(x => x)
                )
            }
        );
        rootElement.ContentTemplate = new DataTemplate<TooManyTabsItem<T>, UIElement>(
            tmti => new TooManyTabsViewSelector<T>(_Parent, tmti)
        );
    }
}