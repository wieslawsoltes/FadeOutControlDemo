using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace FadeOutControlDemo.Controls;

[TemplatePart("PART_TrimmedTextBlock", typeof(TextBlock))]
[TemplatePart("PART_NoTrimTextBlock", typeof(FadeOutTextBlock))]
public class FadeOutTextControl : TemplatedControl
{
	public static readonly StyledProperty<string?> TextProperty =
		AvaloniaProperty.Register<TextBlock, string?>(nameof(Text));

	private TextBlock? _trimmedTextBlock;
	private FadeOutTextBlock? _noTrimTextBlock;

	public string? Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		_trimmedTextBlock = e.NameScope.Find<TextBlock>("PART_TrimmedTextBlock");
		_noTrimTextBlock = e.NameScope.Find<FadeOutTextBlock>("PART_NoTrimTextBlock");

		if (_trimmedTextBlock is not null && _noTrimTextBlock is not null)
		{
			_noTrimTextBlock._trimmedTextBlock = _trimmedTextBlock;
		}
	}
}

public class FadeOutTextBlock : TextBlock
{
	private static readonly IBrush FadeoutOpacityMask = new LinearGradientBrush
	{
		StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
		EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
		GradientStops =
		{
			new GradientStop { Color = Colors.White, Offset = 0 },
			new GradientStop { Color = Colors.White, Offset = 0.7 },
			new GradientStop { Color = Colors.Transparent, Offset = 0.9 }
		}
	}.ToImmutable();

	internal TextBlock? _trimmedTextBlock;

	protected override void RenderTextLayout(DrawingContext context, Point origin)
	{
		if (_trimmedTextBlock is not null)
		{
			var cutOff = _trimmedTextBlock.TextLayout.TextLines[0].HasCollapsed;
			if (cutOff)
			{
				using var b = context.PushOpacityMask(FadeoutOpacityMask, Bounds);
				TextLayout.Draw(context, origin + new Point(TextLayout.OverhangLeading, 0));
			}
			else
			{
				TextLayout.Draw(context, origin + new Point(TextLayout.OverhangLeading, 0));
			}
		}
		else
		{
			base.RenderTextLayout(context, origin);
		}
	}
}
