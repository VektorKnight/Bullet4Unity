using System;
using BulletSharp;
using UnityEngine;
using BM = BulletSharp.Math;

namespace Bullet4Unity {
    /// <summary>
    /// Re-implemented from the BulletSharpUnity3d project
    /// Portions of code credit to Phong13
    /// </summary>
    public class DebugDrawUnity : DebugDraw {
        public override void DrawLine(ref BM.Vector3 from, ref BM.Vector3 to, ref BM.Vector3 fromColor) {
            Gizmos.color = new Color(fromColor.X, fromColor.Y, fromColor.Z);
            Gizmos.DrawLine(from.ToUnity(), to.ToUnity());
        }
        public override void DrawLine(ref BM.Vector3 from, ref BM.Vector3 to, ref BM.Vector3 fromColor, ref BM.Vector3 toColor) {
            Gizmos.color = new Color(fromColor.X, fromColor.Y, fromColor.Z);
            Gizmos.DrawLine(from.ToUnity(), to.ToUnity());
        }
        public override void DrawBox(ref BM.Vector3 bbMin, ref BM.Vector3 bbMax, ref BM.Vector3 color) {
            var b = new Bounds(bbMin.ToUnity(), Vector3.zero);
            b.Encapsulate(bbMax.ToUnity());
            Gizmos.color = new Color(color.X, color.Y, color.Z);
            Gizmos.DrawWireCube(b.center,b.size);
        }

        public override void DrawBox(ref BM.Vector3 bbMin, ref BM.Vector3 bbMax, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var size = (bbMax - bbMin).ToUnity();
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawBox(pos, rot, scale, size,c);
        }
        public override void DrawSphere(ref BM.Vector3 p, float radius, ref BM.Vector3 color) {
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawSphere(p.ToUnity(),Quaternion.identity,Vector3.one, Vector3.one * radius, c);
        }

        public override void DrawSphere(float radius, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawSphere(pos, rot, scale, Vector3.one * radius, c);
        }

        public override void DrawTriangle(ref BM.Vector3 v0, ref BM.Vector3 v1, ref BM.Vector3 v2, ref BM.Vector3 n0, ref BM.Vector3 n1, ref BM.Vector3 n2, ref BM.Vector3 color, float alpha) {
            //todo normals and alpha
            Gizmos.color = new Color(color.X, color.Y, color.Z);
            Gizmos.DrawLine(v0.ToUnity(), v1.ToUnity());
            Gizmos.DrawLine(v1.ToUnity(), v2.ToUnity());
            Gizmos.DrawLine(v2.ToUnity(), v0.ToUnity());
        }
        public override void DrawTriangle(ref BM.Vector3 v0, ref BM.Vector3 v1, ref BM.Vector3 v2, ref BM.Vector3 color, float alpha) {
            Gizmos.color = new Color(color.X, color.Y, color.Z);
            Gizmos.DrawLine(v0.ToUnity(), v1.ToUnity());
            Gizmos.DrawLine(v1.ToUnity(), v2.ToUnity());
            Gizmos.DrawLine(v2.ToUnity(), v0.ToUnity());
        }
        public override void DrawContactPoint(ref BM.Vector3 pointOnB, ref BM.Vector3 normalOnB, float distance, int lifeTime, ref BM.Vector3 color) {
            Gizmos.color = new Color(color.X, color.Y, color.Z);
            Gizmos.DrawWireSphere(pointOnB.ToUnity(), .2f);
        }
        public override void ReportErrorWarning(String warningString) {
            Debug.LogError(warningString);
        }
        public override void Draw3dText(ref BM.Vector3 location, String textString) {
            Debug.LogError("Not implemented");
        }
        
        DebugDrawModes _debugMode;
        public override DebugDrawModes DebugMode {
            get { return _debugMode; }
            set { _debugMode = value; }
        }

        public override void DrawAabb(ref BM.Vector3 from, ref BM.Vector3 to, ref BM.Vector3 color) {
            DrawBox(ref from, ref to, ref color);
        }
        public override void DrawTransform(ref BM.Matrix trans, float orthoLen) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var p1 = pos + rot * Vector3.up * orthoLen;
            var p2 = pos - rot * Vector3.up * orthoLen;
            Gizmos.DrawLine(p1, p2);
            p1 = pos + rot * Vector3.right * orthoLen;
            p2 = pos - rot * Vector3.right * orthoLen;
            Gizmos.DrawLine(p1, p2);
            p1 = pos + rot * Vector3.forward * orthoLen;
            p2 = pos - rot * Vector3.forward * orthoLen;
            Gizmos.DrawLine(p1, p2);
        }
        public override void DrawArc(ref BM.Vector3 center, ref BM.Vector3 normal, ref BM.Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle,
            ref BM.Vector3 color, bool drawSect)
        {
            Debug.LogError("Not implemented");
        }
        public override void DrawArc(ref BM.Vector3 center, ref BM.Vector3 normal, ref BM.Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle,
            ref BM.Vector3 color, bool drawSect, float stepDegrees)
        {
            var col = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawArc(center.ToUnity(), normal.ToUnity(), axis.ToUnity(), radiusA, radiusB, minAngle, maxAngle, col, drawSect, stepDegrees);
        }
        public override void DrawSpherePatch(ref BM.Vector3 center, ref BM.Vector3 up, ref BM.Vector3 axis, float radius,
            float minTh, float maxTh, float minPs, float maxPs, ref BM.Vector3 color)
        {
            Debug.LogError("Not implemented");
            
        }
        public override void DrawSpherePatch(ref BM.Vector3 center, ref BM.Vector3 up, ref BM.Vector3 axis, float radius,
            float minTh, float maxTh, float minPs, float maxPs, ref BM.Vector3 color, float stepDegrees)
        {
            Debug.LogError("Not implemented");
        }
        public override void DrawCapsule(float radius, float halfHeight, int upAxis, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawCapsule(pos, rot, scale, radius, halfHeight, upAxis, c);
        }
        public override void DrawCylinder(float radius, float halfHeight, int upAxis, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawCylinder(pos,rot,scale,radius,halfHeight,upAxis, c);
        }
        public override void DrawCone(float radius, float height, int upAxis, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawCone(pos, rot, scale, radius, height, upAxis, c);
        }
        public override void DrawPlane(ref BM.Vector3 planeNormal, float planeConst, ref BM.Matrix trans, ref BM.Vector3 color) {
            var pos = BulletExtensionMethods.ExtractTranslationFromMatrix(ref trans);
            var rot = BulletExtensionMethods.ExtractRotationFromMatrix(ref trans);
            var scale = BulletExtensionMethods.ExtractScaleFromMatrix(ref trans);
            var c = new Color(color.X, color.Y, color.Z);
            BUtility.DebugDrawPlane(pos, rot, scale, planeNormal.ToUnity(), planeConst, c);
        }
    }
}
