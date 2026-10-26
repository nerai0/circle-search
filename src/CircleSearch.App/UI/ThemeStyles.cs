using System.Windows;
using System.Windows.Markup;

namespace CircleSearch.UI;

/// <summary>
/// Централизованные неявные стили базовых WPF-контролов (TextBox, CheckBox как переключатель, ComboBox, ScrollBar).
/// Все цвета берутся через DynamicResource из токенов <see cref="ThemeManager"/>, поэтому тема переключается на лету.
/// </summary>
public static class ThemeStyles
{
    private static bool _loaded;

    public static void EnsureLoaded(System.Windows.Application app)
    {
        if (_loaded) return;
        var dictionary = (ResourceDictionary)XamlReader.Parse(StylesXaml);
        app.Resources.MergedDictionaries.Add(dictionary);
        _loaded = true;
    }

    private const string StylesXaml = """
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- Минималистичный scrollbar -->
  <Style x:Key="ThinScrollThumb" TargetType="Thumb">
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="Thumb">
          <Border CornerRadius="3" Background="{DynamicResource TextSecondaryBrush}" Opacity="0.4" Margin="1,0"/>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>
  <Style x:Key="ThinScrollPage" TargetType="RepeatButton">
    <Setter Property="Focusable" Value="False"/>
    <Setter Property="IsTabStop" Value="False"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="RepeatButton">
          <Border Background="Transparent"/>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>
  <Style TargetType="ScrollBar">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Width" Value="8"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="ScrollBar">
          <Grid Background="Transparent">
            <Track x:Name="PART_Track" IsDirectionReversed="True">
              <Track.DecreaseRepeatButton>
                <RepeatButton Command="ScrollBar.PageUpCommand" Style="{StaticResource ThinScrollPage}"/>
              </Track.DecreaseRepeatButton>
              <Track.Thumb>
                <Thumb Style="{StaticResource ThinScrollThumb}"/>
              </Track.Thumb>
              <Track.IncreaseRepeatButton>
                <RepeatButton Command="ScrollBar.PageDownCommand" Style="{StaticResource ThinScrollPage}"/>
              </Track.IncreaseRepeatButton>
            </Track>
          </Grid>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

  <!-- Поле ввода без рамки: состояние показывается тоном поверхности -->
  <Style TargetType="TextBox">
    <Setter Property="Foreground" Value="{DynamicResource TextPrimaryBrush}"/>
    <Setter Property="Background" Value="{DynamicResource BgElevatedBrush}"/>
    <Setter Property="CaretBrush" Value="{DynamicResource TextPrimaryBrush}"/>
    <Setter Property="SelectionBrush" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="Padding" Value="10,8"/>
    <Setter Property="FontSize" Value="13"/>
    <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="TextBox">
          <Border x:Name="Bd" Background="{TemplateBinding Background}" CornerRadius="10"
                  Padding="{TemplateBinding Padding}">
            <ScrollViewer x:Name="PART_ContentHost" Focusable="False" VerticalAlignment="Center"/>
          </Border>
          <ControlTemplate.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
              <Setter TargetName="Bd" Property="Background" Value="{DynamicResource BgHoverBrush}"/>
            </Trigger>
            <Trigger Property="IsKeyboardFocused" Value="True">
              <Setter TargetName="Bd" Property="Background" Value="{DynamicResource BgHoverBrush}"/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

  <!-- CheckBox как переключатель -->
  <Style TargetType="CheckBox">
    <Setter Property="Foreground" Value="{DynamicResource TextPrimaryBrush}"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="CheckBox">
          <Grid Background="Transparent" SnapsToDevicePixels="True">
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            <Border x:Name="Track" Width="36" Height="20" CornerRadius="10"
                    Background="{DynamicResource BgHoverBrush}" VerticalAlignment="Center">
              <Ellipse x:Name="Thumb" Width="14" Height="14" Fill="{DynamicResource TextSecondaryBrush}"
                       HorizontalAlignment="Left" Margin="3,0,0,0"/>
            </Border>
            <ContentPresenter Grid.Column="1" Margin="10,0,0,0" VerticalAlignment="Center"
                              RecognizesAccessKey="True"/>
          </Grid>
          <ControlTemplate.Triggers>
            <Trigger Property="IsChecked" Value="True">
              <Setter TargetName="Track" Property="Background" Value="{DynamicResource PrimaryBrush}"/>
              <Setter TargetName="Thumb" Property="HorizontalAlignment" Value="Right"/>
              <Setter TargetName="Thumb" Property="Margin" Value="0,0,3,0"/>
              <Setter TargetName="Thumb" Property="Fill" Value="White"/>
            </Trigger>
            <Trigger Property="IsEnabled" Value="False">
              <Setter Property="Opacity" Value="0.5"/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

  <!-- ComboBox -->
  <Style TargetType="ComboBoxItem">
    <Setter Property="Foreground" Value="{DynamicResource TextPrimaryBrush}"/>
    <Setter Property="Padding" Value="10,8"/>
    <Setter Property="FontSize" Value="13"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="ComboBoxItem">
          <Border x:Name="Bd" Background="Transparent" CornerRadius="8" Padding="{TemplateBinding Padding}">
            <ContentPresenter/>
          </Border>
          <ControlTemplate.Triggers>
            <Trigger Property="IsHighlighted" Value="True">
              <Setter TargetName="Bd" Property="Background" Value="{DynamicResource BgHoverBrush}"/>
            </Trigger>
            <Trigger Property="IsSelected" Value="True">
              <Setter TargetName="Bd" Property="Background" Value="{DynamicResource PrimaryBrush}"/>
              <Setter Property="Foreground" Value="White"/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>
  <Style TargetType="ComboBox">
    <Setter Property="Foreground" Value="{DynamicResource TextPrimaryBrush}"/>
    <Setter Property="Padding" Value="10,8"/>
    <Setter Property="FontSize" Value="13"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    <Setter Property="ScrollViewer.HorizontalScrollBarVisibility" Value="Disabled"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="ComboBox">
          <Grid>
            <ToggleButton x:Name="Toggle" Focusable="False" ClickMode="Press"
                          IsChecked="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}">
              <ToggleButton.Template>
                <ControlTemplate TargetType="ToggleButton">
                  <Border x:Name="Bd" Background="{DynamicResource BgElevatedBrush}" CornerRadius="10">
                    <Path Data="M0,0 L5,5 L10,0" Stroke="{DynamicResource TextSecondaryBrush}" StrokeThickness="1.6"
                          Width="10" Height="5" HorizontalAlignment="Right" VerticalAlignment="Center"
                          Margin="0,0,14,0"/>
                  </Border>
                  <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                      <Setter TargetName="Bd" Property="Background" Value="{DynamicResource BgHoverBrush}"/>
                    </Trigger>
                  </ControlTemplate.Triggers>
                </ControlTemplate>
              </ToggleButton.Template>
            </ToggleButton>
            <ContentPresenter IsHitTestVisible="False" Margin="{TemplateBinding Padding}"
                              Content="{TemplateBinding SelectionBoxItem}"
                              ContentTemplate="{TemplateBinding SelectionBoxItemTemplate}"
                              VerticalAlignment="Center" HorizontalAlignment="Left"/>
            <Popup x:Name="PART_Popup" Placement="Bottom" IsOpen="{TemplateBinding IsDropDownOpen}"
                   AllowsTransparency="True" Focusable="False" PopupAnimation="None">
              <Grid MinWidth="{TemplateBinding ActualWidth}" MaxHeight="{TemplateBinding MaxDropDownHeight}"
                    Margin="0,4,0,0">
                <Border Background="{DynamicResource BgElevatedBrush}" CornerRadius="10" Padding="4">
                  <ScrollViewer>
                    <ItemsPresenter KeyboardNavigation.DirectionalNavigation="Contained"/>
                  </ScrollViewer>
                </Border>
              </Grid>
            </Popup>
          </Grid>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>
</ResourceDictionary>
""";
}
