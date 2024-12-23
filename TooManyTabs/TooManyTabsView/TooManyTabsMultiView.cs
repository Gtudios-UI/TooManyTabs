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
        rootElement.OrientationBinding = OneWay(TooManyTabsMultiItemProperty.SelectPath(x => x.TabViewOrientationProperty));
        rootElement.Header = new ContentBundleControl {
            ContentBundle = new ContentBundle<TooManyTabsMultiItem<T>, UIElement>(TooManyTabsMultiItem)
            {
                ContentBinding = OneWay(TooManyTabsMultiItemProperty),
                ContentTemplateBinding = OneWay(_Parent.TabViewHeaderProperty)
            }
        };
        rootElement.AddTabButtonVisibilityBinding = OneWay(_Parent.AddTabButtonVisibilityProperty);
        rootElement.AddTabButtonClicked += delegate
        {
            _Parent.OnAddTabButtonClicked(TooManyTabsMultiItem);
        };
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
        rootElement.ItemTemplateBinding = OneWay(
            from t in _Parent.TabStyleTemplateProperty
            select 
                (IDataTemplate<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>>)
                new TemplateLink<T>(t)
        );
        rootElement.TabClosing += _Parent.OnTabCloseButtonClicked;
        rootElement.ContentTemplate = new DataTemplate<TooManyTabsItem<T>, UIElement>(
            tmti => new TooManyTabsViewSelector<T>(_Parent, tmti)
        );
        Initialized?.Invoke(rootElement);
    }
    public event Action<TabView<TooManyTabsItem<T>>>? Initialized;
    //static DataTemplate<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>> TabTemplate { get; } = new(tmti => 
    //);
}
readonly struct TemplateLink<T>(IDataTemplate<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>> t) : IDataTemplate<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>>
{
    public IDataTemplateGeneratedValue<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>> Generate(IReadOnlyBinding<TooManyTabsItem<T>> source)
    {
        var gen = t.Generate(source.SelectPath(x => x.TabStyleProperty));
        return new GeneratedValue<T>(this, gen, source);
    }

    public void NotifyRecycle(IDataTemplateGeneratedValue<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>> recycledItem)
    {
        if (recycledItem is GeneratedValue<T> g)
            t.NotifyRecycle(g.Original);
    }
}
class GeneratedValue<T>(TemplateLink<T> parent, IDataTemplateGeneratedValue<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>> g, IReadOnlyBinding<TooManyTabsItem<T>> originalBinding) : IDataTemplateGeneratedValue<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>>
{
    public IDataTemplateGeneratedValue<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>> Original { get; } = g;
    IReadOnlyBinding<TooManyTabsItem<T>> _Binding = originalBinding;
    public IReadOnlyBinding<TooManyTabsItem<T>> Binding {
        get => _Binding;
        set
        {
            _Binding = value;
            Original.Binding = value.SelectPath(x => x.TabStyleProperty);
        }
    }

    public IDataTemplate<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>> Template => parent;

    TabItem<TooManyTabsItem<T>> IDataTemplateGeneratedValue<TooManyTabsItem<T>, TabItem<TooManyTabsItem<T>>>.GeneratedValue => Original.GeneratedValue;

    public void Recycle() => Original.Recycle();
}