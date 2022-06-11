using System;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;

namespace DG.Tweening.Core
{
	public class TweenerCore<T1, T2, TPlugOptions> : Tweener where TPlugOptions : struct, IPlugOptions
	{
		public T2 startValue;

		public T2 endValue;

		public T2 changeValue;

		public TPlugOptions plugOptions;

		public DOGetter<T1> getter;

		public DOSetter<T1> setter;

		internal ABSTweenPlugin<T1, T2, TPlugOptions> tweenPlugin;

		private const string _TxtCantChangeSequencedValues = "You cannot change the values of a tween contained inside a Sequence";

		internal TweenerCore()
		{
			typeofT1 = typeof(T1);
			typeofT2 = typeof(T2);
			typeofTPlugOptions = typeof(TPlugOptions);
			tweenType = TweenType.Tweener;
			Reset();
		}

		public override Tweener ChangeStartValue(object newStartValue, float newDuration = -1f)
		{
			if (isSequenced)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning("You cannot change the values of a tween contained inside a Sequence");
				}
				return this;
			}
			Type type = newStartValue.GetType();
			if (type != typeofT2)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning(string.Concat("ChangeStartValue: incorrect newStartValue type (is ", type, ", should be ", typeofT2, ")"));
				}
				return this;
			}
			return Tweener.DoChangeStartValue(this, (T2)newStartValue, newDuration);
		}

		public override Tweener ChangeEndValue(object newEndValue, bool snapStartValue)
		{
			return ChangeEndValue(newEndValue, -1f, snapStartValue);
		}

		public override Tweener ChangeEndValue(object newEndValue, float newDuration = -1f, bool snapStartValue = false)
		{
			if (isSequenced)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning("You cannot change the values of a tween contained inside a Sequence");
				}
				return this;
			}
			Type type = newEndValue.GetType();
			if (type != typeofT2)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning(string.Concat("ChangeEndValue: incorrect newEndValue type (is ", type, ", should be ", typeofT2, ")"));
				}
				return this;
			}
			return Tweener.DoChangeEndValue(this, (T2)newEndValue, newDuration, snapStartValue);
		}

		public override Tweener ChangeValues(object newStartValue, object newEndValue, float newDuration = -1f)
		{
			if (isSequenced)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning("You cannot change the values of a tween contained inside a Sequence");
				}
				return this;
			}
			Type type = newStartValue.GetType();
			Type type2 = newEndValue.GetType();
			if (type != typeofT2)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning(string.Concat("ChangeValues: incorrect value type (is ", type, ", should be ", typeofT2, ")"));
				}
				return this;
			}
			if (type2 != typeofT2)
			{
				if (Debugger.logPriority >= 1)
				{
					Debugger.LogWarning(string.Concat("ChangeValues: incorrect value type (is ", type2, ", should be ", typeofT2, ")"));
				}
				return this;
			}
			return Tweener.DoChangeValues(this, (T2)newStartValue, (T2)newEndValue, newDuration);
		}

		internal override Tweener SetFrom(bool relative)
		{
			tweenPlugin.SetFrom(this, relative);
			hasManuallySetStartValue = true;
			return this;
		}

		internal sealed override void Reset()
		{
			base.Reset();
			if (tweenPlugin != null)
			{
				tweenPlugin.Reset(this);
			}
			plugOptions.Reset();
			getter = null;
			setter = null;
			hasManuallySetStartValue = false;
			isFromAllowed = true;
		}

		internal override bool Validate()
		{
			try
			{
				getter();
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal override float UpdateDelay(float elapsed)
		{
			return Tweener.DoUpdateDelay(this, elapsed);
		}

		internal override bool Startup()
		{
			return Tweener.DoStartup(this);
		}

		internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice)
		{
			float elapsed = (useInversePosition ? (duration - base.position) : base.position);
			if (DOTween.useSafeMode)
			{
				try
				{
					tweenPlugin.EvaluateAndApply(plugOptions, this, base.isRelative, getter, setter, elapsed, startValue, changeValue, duration, useInversePosition, updateNotice);
				}
				catch
				{
					return true;
				}
			}
			else
			{
				tweenPlugin.EvaluateAndApply(plugOptions, this, base.isRelative, getter, setter, elapsed, startValue, changeValue, duration, useInversePosition, updateNotice);
			}
			return false;
		}
	}
}
