//
// Extra.cs: API extensions for Cocos2D binding to Mono.
//
// Author:
//   Stephane Delcroix
//
// Copyright 2012, 2013 S. Delcroix
//

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace MonoTouch.Cocos2D {
	// Use this for synchronous operations
	[Register ("__My_NSActionDispatcher")]
	internal class NSActionDispatcher : NSObject {

		public static Selector Selector = new Selector ("apply");

		NSAction action;

		public NSActionDispatcher (NSAction action)
		{
			this.action = action;
		}

		[Export ("apply")]
		[Preserve (Conditional = true)]
		public void Apply ()
		{
			action ();
		}
	}
	
	[Register ("__My_NSActionDispatcherWithNode")]
	internal class NSActionDispatcherWithNode : NSObject {

		public static Selector Selector = new Selector ("apply:");

		Action<CCNode> action;

		public NSActionDispatcherWithNode (Action<CCNode> action)
		{
			this.action = action;
		}

		[Export ("apply:")]
		[Preserve (Conditional = true)]
		public void Apply (CCNode node)
		{
			action (node);
		}
	}

	[Register ("__My_NSActionDispatcherWithFloat")]
	internal class NSActionDispatcherWithFloat : NSObject {

		public static Selector Selector = new Selector ("apply:");

		Action<float> action;

		public NSActionDispatcherWithFloat (Action<float> action)
		{
			this.action = action;
		}

		[Export ("apply:")]
		[Preserve (Conditional = true)]
		public void Apply (float timer)
		{
			action (timer);
		}
	}
	
	public partial class CCNode {
		static CCScheduler scheduler = CCDirector.SharedDirector.Scheduler;
		public const uint RepeatForever = uint.MaxValue - 1;

		public void Schedule (Action<float> callback, float interval=0, uint repeat=RepeatForever, float delay=0)
		{
			scheduler.ScheduleSelector(NSActionDispatcherWithFloat.Selector, new NSActionDispatcherWithFloat(callback), interval, !IsRunning, repeat, delay);
		}

		public void ScheduleOnce (Action<float> callback, float delay)
		{
			Schedule (callback, repeat:0, delay:delay);
		}
	}

	public partial class CCScheduler {
		public const uint RepeatForever = uint.MaxValue - 1;
		public NSObject Schedule (Action<float> callback, float interval=0, bool paused=false, uint repeat=RepeatForever, float delay=0)
		{
			var token = new NSActionDispatcherWithFloat (callback);
			ScheduleSelector (NSActionDispatcherWithFloat.Selector, token, interval, paused, repeat, delay);
			return token;
		}
		
	}

	public partial class CCMenuItemLabel {
		public CCMenuItemLabel (NSCallbackWithSender callback) : base (callback)
		{
		}

	}
	public partial class CCMenuItemImage {
		public CCMenuItemImage (string normalImageFile, string selectedImageFile, NSCallbackWithSender callback) : this (normalImageFile, selectedImageFile, null, callback)
		{
		} 
	}

	public partial class CCMenu {
		void AlignItemsInColumns (params NSNumber[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException ("columns");

			var pNativeArr = Marshal.AllocHGlobal(columns.Length * IntPtr.Size);
			for (var i =1; i<columns.Length;++i)
				Marshal.WriteIntPtr (pNativeArr, (i-1)*IntPtr.Size, columns[i].Handle);

			//Null termination
			Marshal.WriteIntPtr (pNativeArr, (columns.Length-1)*IntPtr.Size, IntPtr.Zero);

			AlignItemsInColumns (columns[0], pNativeArr);
			Marshal.FreeHGlobal(pNativeArr);
		}
		
		void AlignItemsInRows (params NSNumber[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException ("rows");

			var pNativeArr = Marshal.AllocHGlobal(rows.Length * IntPtr.Size);
			for (var i =1; i<rows.Length;++i)
				Marshal.WriteIntPtr (pNativeArr, (i-1)*IntPtr.Size, rows[i].Handle);

			//Null termination
			Marshal.WriteIntPtr (pNativeArr, (rows.Length-1)*IntPtr.Size, IntPtr.Zero);

			AlignItemsInColumns (rows[0], pNativeArr);
			Marshal.FreeHGlobal(pNativeArr);
		}
		
	}

	public partial class CCCallFunc {
		public CCCallFunc (NSAction callback) : this (new NSActionDispatcher(callback), NSActionDispatcher.Selector)
		{
		}
	}

	public partial class CCCallFuncN {
		public CCCallFuncN (Action<CCNode> callback) : this (new NSActionDispatcherWithNode(callback), NSActionDispatcherWithNode.Selector)
		{
		}
	}

	public partial class CCSpriteBatchNode {
		const int DEFAULTCAPACITY = 29; 
		public CCSpriteBatchNode (CCTexture2D texture) : this (texture, DEFAULTCAPACITY)
		{
		}

		public CCSpriteBatchNode (string filename) : this (filename, DEFAULTCAPACITY)
		{
		}
	}

	public partial class CCPointArray {
		public PointF this [int index] {
			get {
				return GetControlPoint (index);
			}
			set {
				Replace (value, index);
			}
		}
	}

	public partial class CCCardianSpline {
		[DllImport ("__Internal", EntryPoint="ccCardinalSplineAt")]
		public extern static PointF GetPosition (PointF p0, PointF p1, PointF p2, PointF p3, float tension, float time);
	}

	public partial class CCDirector {
		[Obsolete ("Poorly named, use PushScene instead")]
		public void Push (CCScene scene)
		{
			PushScene (scene);
		}
	}

	public partial class CCLabelBMFont {
		public float Width {
			set {
				SetWidth (value);
			}
		}
	}

	public partial class CCTimer {
		public CCTimer (NSAction target) : this (new NSActionDispatcher (target), NSActionDispatcher.Selector)
		{
		}
	}

	public partial class CCTexture2D {
		[Obsolete ("Obsolete since 2.1. Use CCTexture2D (string text, string fontName, float fontSize, UITextAlignment alignmenr, CCVerticalTextAlignment vertAlignmenr) instead.")]
#if MONOMAC
		public CCTexture2D (string text, SizeF dimensions, NSTextAlignment alignment, CCVerticalTextAlignment vertAlignment, string fontName, float fontSize) : this (text, fontName, fontSize, dimensions, alignment, vertAlignment)
#else
		public CCTexture2D (string text, SizeF dimensions, UITextAlignment alignment, CCVerticalTextAlignment vertAlignment, string fontName, float fontSize) : this (text, fontName, fontSize, dimensions, alignment, vertAlignment)
#endif
		{
		}
	}

	public partial class CCLabelTTF {
		[Obsolete ("Obsolete since 2.1. Use CCLabelTTF (string label, string fontName, float fontSize, SizeF dimensions, UITextAlignment alignment, UILineBreakMode lineBreakMode) instead.")]
#if MONOMAC
		public CCLabelTTF (string label, SizeF dimensions, NSTextAlignment alignment, NSLineBreakMode lineBreakMode, string fontName, float fontSize) : this (label, fontName, fontSize, dimensions, alignment, lineBreakMode)
#else
		public CCLabelTTF (string label, SizeF dimensions, UITextAlignment alignment, UILineBreakMode lineBreakMode, string fontName, float fontSize) : this (label, fontName, fontSize, dimensions, alignment, lineBreakMode)
#endif
		{
		}

		[Obsolete ("Obsolete since 2.1, Use CCLabelTTF (string label, string fontName, float fontSize, SizeF dimensions, UITextAlignment alignment) instead.")]
#if MONOMAC
		public CCLabelTTF (string label, SizeF dimensions, NSTextAlignment alignment, string fontName, float fontSize) : this (label, fontName, fontSize, dimensions, alignment)
#else
		public CCLabelTTF (string label, SizeF dimensions, UITextAlignment alignment, string fontName, float fontSize) : this (label, fontName, fontSize, dimensions, alignment)
#endif
		{
		}
	}
#if ENABLE_CHIPMUNK_INTEGRATION
	public partial class CCPhysicsSprite {
		public Chipmunk.Body Body {
			get { return new Chipmunk.Body (BodyPtr); }
			set { BodyPtr = value.Handle.Handle; }
		} 

		public PointF Position {
			get {
				if (BodyPtr == IntPtr.Zero)
					throw new InvalidOperationException ("You can't get the Position if the Body isn't set");
				return PositionInt;
			}
			set {
				if (BodyPtr == IntPtr.Zero)
					throw new InvalidOperationException ("You can't set the Position if the Body isn't set");
				PositionInt = value;
			}
		}
	}

	public partial class CCPhysicsDebugNode {
		public static CCPhysicsDebugNode DebugNode (Chipmunk.Space space) 
		{
			return DebugNode (space.Handle.Handle);
		}
	}
#endif
}	
