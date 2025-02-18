﻿using CoreGraphics;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;

namespace MauiShellCustomization;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

class CustomShellItemRenderer(IShellContext context) : ShellItemRenderer(context)
{
	UIButton? middleView;

	public override async void ViewWillLayoutSubviews()
	{
		base.ViewWillLayoutSubviews();
		if (View is not null && ShellItem is CustomTabBar { CenterViewVisible: true } tabbar)
		{
			if (middleView is not null)
			{
				middleView.RemoveFromSuperview();
			}

			if (middleView is null)
			{
				var context = tabbar.Window?.Page?.Handler?.MauiContext ?? Application.Current?.Windows.LastOrDefault()?.Page?.Handler?.MauiContext;
				var image = await tabbar.CenterViewImageSource.GetPlatformImageAsync(context!);

				middleView = new UIButton(UIButtonType.Custom);
				middleView.BackgroundColor = tabbar.CenterViewBackgroundColor?.ToPlatform();
				middleView.Frame = new CGRect(CGPoint.Empty, new CGSize(70, 70));
				if (image is not null)
				{
					middleView.SetImage(image.Value, UIControlState.Normal);
					middleView.Frame = new CGRect(CGPoint.Empty, image.Value.Size);
				}

				middleView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin |
											  UIViewAutoresizing.FlexibleLeftMargin |
											  UIViewAutoresizing.FlexibleBottomMargin;
				middleView.Layer.CornerRadius = middleView.Frame.Width / 2;
				middleView.Layer.MasksToBounds = false;

				middleView.TouchUpInside += (_, _) =>
				{
					tabbar.CenterViewCommand?.Execute(null);
				};
			}

			middleView.Center = new CGPoint(View.Bounds.GetMidX(), TabBar.Frame.Top - middleView.Frame.Height / 2);

			View.AddSubview(middleView);
		}
	}
}