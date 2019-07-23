using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using J.External;

namespace J
{

	public class CurveGenerator : MonoBehaviour
	{
	    public AnimationCurve curve;    // for preview

	    protected delegate double EasingFunction(double time,double min,double max,double duration);

		public static AnimationCurve GenerateCurve (CurveType type)
		{
			MethodInfo methodInfo = (typeof(PennerDoubleAnimation)).GetMethod (type.ToString ());
			Delegate foo = Delegate.CreateDelegate (typeof(EasingFunction), methodInfo);
			return GenerateCurve (foo as EasingFunction, 30);
		}

	    protected static AnimationCurve GenerateCurve(EasingFunction easingFunction, int resolution)
	    {
	        var curve = new AnimationCurve();
	        for (var i = 0; i < resolution; ++i)
	        {
	            var time = i / (resolution - 1f);
	            var value = (float)easingFunction(time, 0.0, 1.0, 1.0);
	            var key = new Keyframe(time, value);
	            curve.AddKey(key);
	        }
	        for (var i = 0; i < resolution; ++i)
	        {
	            curve.SmoothTangents(i, 0f);
	        }
	        return curve;
	    }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/EasingCurves")]
	    protected static void CreateAsset()
	    {
	        var curvePresetLibraryType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
	        var library = ScriptableObject.CreateInstance(curvePresetLibraryType);

			addCurve(library, PennerDoubleAnimation.Linear, 2, CurveType.Linear);
			addCurve(library, PennerDoubleAnimation.SineEaseIn, 15, CurveType.SineEaseIn);
			addCurve(library, PennerDoubleAnimation.QuadEaseIn, 15, CurveType.QuadEaseIn);
			addCurve(library, PennerDoubleAnimation.CubicEaseIn, 15, CurveType.CubicEaseIn);
			addCurve(library, PennerDoubleAnimation.QuartEaseIn, 15, CurveType.QuartEaseIn);
			addCurve(library, PennerDoubleAnimation.QuintEaseIn, 15, CurveType.QuintEaseIn);
			addCurve(library, PennerDoubleAnimation.ExpoEaseIn, 15, CurveType.ExpoEaseIn);
			addCurve(library, PennerDoubleAnimation.CircEaseIn, 15, CurveType.CircEaseIn);
			addCurve(library, PennerDoubleAnimation.BackEaseIn, 30, CurveType.BackEaseIn);
			addCurve(library, PennerDoubleAnimation.ElasticEaseIn, 30, CurveType.ElasticEaseIn);
			addCurve(library, PennerDoubleAnimation.BounceEaseIn, 30, CurveType.BounceEaseIn);

			addCurve(library, PennerDoubleAnimation.SineEaseOut, 15, CurveType.SineEaseOut);
			addCurve(library, PennerDoubleAnimation.QuadEaseOut, 15, CurveType.QuadEaseOut);
			addCurve(library, PennerDoubleAnimation.CubicEaseOut, 15, CurveType.CubicEaseOut);
			addCurve(library, PennerDoubleAnimation.QuartEaseOut, 15, CurveType.QuartEaseOut);
			addCurve(library, PennerDoubleAnimation.QuintEaseOut, 15, CurveType.QuintEaseOut);
			addCurve(library, PennerDoubleAnimation.ExpoEaseOut, 15, CurveType.ExpoEaseOut);
			addCurve(library, PennerDoubleAnimation.CircEaseOut, 15, CurveType.CircEaseOut);
			addCurve(library, PennerDoubleAnimation.BackEaseOut, 30, CurveType.BackEaseOut);
			addCurve(library, PennerDoubleAnimation.ElasticEaseOut, 30, CurveType.ElasticEaseOut);
			addCurve(library, PennerDoubleAnimation.BounceEaseOut, 30, CurveType.BounceEaseOut);

			addCurve(library, PennerDoubleAnimation.SineEaseInOut, 15, CurveType.SineEaseInOut);
			addCurve(library, PennerDoubleAnimation.QuadEaseInOut, 15, CurveType.QuadEaseInOut);
			addCurve(library, PennerDoubleAnimation.CubicEaseInOut, 15, CurveType.CubicEaseInOut);
			addCurve(library, PennerDoubleAnimation.QuartEaseInOut, 15, CurveType.QuartEaseInOut);
			addCurve(library, PennerDoubleAnimation.QuintEaseInOut, 15, CurveType.QuintEaseInOut);
			addCurve(library, PennerDoubleAnimation.ExpoEaseInOut, 15, CurveType.ExpoEaseInOut);
			addCurve(library, PennerDoubleAnimation.CircEaseInOut, 15, CurveType.CircEaseInOut);
			addCurve(library, PennerDoubleAnimation.BackEaseInOut, 30, CurveType.BackEaseInOut);
			addCurve(library, PennerDoubleAnimation.ElasticEaseInOut, 30, CurveType.ElasticEaseInOut);
			addCurve(library, PennerDoubleAnimation.BounceEaseInOut, 30, CurveType.BounceEaseInOut);

			addCurve(library, PennerDoubleAnimation.SineEaseOutIn, 15, CurveType.SineEaseOutIn);
			addCurve(library, PennerDoubleAnimation.QuadEaseOutIn, 15, CurveType.QuadEaseOutIn);
			addCurve(library, PennerDoubleAnimation.CubicEaseOutIn, 15, CurveType.CubicEaseOutIn);
			addCurve(library, PennerDoubleAnimation.QuartEaseOutIn, 15, CurveType.QuartEaseOutIn);
			addCurve(library, PennerDoubleAnimation.QuintEaseOutIn, 15, CurveType.QuintEaseOutIn);
			addCurve(library, PennerDoubleAnimation.ExpoEaseOutIn, 15, CurveType.ExpoEaseOutIn);
			addCurve(library, PennerDoubleAnimation.CircEaseOutIn, 15, CurveType.CircEaseOutIn);
			addCurve(library, PennerDoubleAnimation.BackEaseOutIn, 30, CurveType.BackEaseOutIn);
			addCurve(library, PennerDoubleAnimation.ElasticEaseOutIn, 30, CurveType.ElasticEaseOutIn);
			addCurve(library, PennerDoubleAnimation.BounceEaseOutIn, 30, CurveType.BounceEaseOutIn);


			// ::: Custom Curves :::
			addCurve(library, PennerDoubleAnimation.SineUpDown, 15, CurveType.SineUpDown);
			addCurve(library, PennerDoubleAnimation.SineDownUp, 15, CurveType.SineDownUp);



			if (!AssetDatabase.IsValidFolder ("Assets/Editor"))
				AssetDatabase.CreateFolder ("Assets", "Editor");
			AssetDatabase.CreateAsset(library, "Assets/Editor/Curves.curves");
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	    }
#endif

        protected static void addCurve(object library, EasingFunction easingFunction, int resolution, CurveType type)
	    {
	        var curvePresetLibraryType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
	        var addMehtod = curvePresetLibraryType.GetMethod("Add");
	        addMehtod.Invoke(library, new object[]
	        {
	            GenerateCurve(easingFunction, resolution),
				type.ToString()
	        });
	    }
			

	}

	public enum CurveType
	{
		Linear			= 0,
		SineEaseIn		= 1, 
		QuadEaseIn      = 2,
		CubicEaseIn     = 3,
		QuartEaseIn     = 4,
		QuintEaseIn     = 5,
		ExpoEaseIn      = 6,
		CircEaseIn      = 7,
		BackEaseIn      = 8,
		ElasticEaseIn   = 9,
		BounceEaseIn    = 10,
		SineEaseOut     = 11,
		QuadEaseOut     = 12,
		CubicEaseOut    = 13,
		QuartEaseOut    = 14,
		QuintEaseOut    = 15,
		ExpoEaseOut     = 16,
		CircEaseOut     = 17,
		BackEaseOut     = 18,
		ElasticEaseOut  = 19,
		BounceEaseOut   = 20,
		SineEaseInOut   = 21,
		QuadEaseInOut   = 22,
		CubicEaseInOut  = 23,
		QuartEaseInOut  = 24,
		QuintEaseInOut  = 25,
		ExpoEaseInOut   = 26,
		CircEaseInOut   = 27,
		BackEaseInOut   = 28,
		ElasticEaseInOut= 29,
		BounceEaseInOut = 30,
		SineEaseOutIn   = 31,
		QuadEaseOutIn   = 32,
		CubicEaseOutIn  = 33,
		QuartEaseOutIn  = 34,
		QuintEaseOutIn  = 35,
		ExpoEaseOutIn   = 36,
		CircEaseOutIn   = 37,
		BackEaseOutIn   = 38,
		ElasticEaseOutIn= 39,
		BounceEaseOutIn = 40,
		SineUpDown      = 41,
		SineDownUp      = 42
	}
}

	

