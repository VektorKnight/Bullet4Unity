using UnityEngine;
using System.Collections;
using BulletSharp;

namespace Bullet4Unity {
    public class BulletDebugHelper {
        public const float Two_PI = 6.283185307179586232f;
        public const float RADS_PER_DEG = Two_PI / 360.0f;
        public const float SQRT12 = 0.7071067811865475244008443621048490f;

        public static void DebugDrawRope(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 begin, Vector3 end, int res, Color color) {
            Gizmos.color = color;
            var matrix = Matrix4x4.TRS(position, rotation, scale);
            var p1 = matrix.MultiplyPoint(begin);
            var p2 = matrix.MultiplyPoint(end);
            var r = res + 2;

            var deltaX = new Vector3(0.05f, 0.05f, 0);
            var deltaZ = new Vector3(0, 0.05f, 0.05f);
            for (var i = 0; i < r; i++) {
                Gizmos.color = color;
                var t = i * 1.0f / (r - 1);
                var tNext = (i + 1) * 1.0f / (r - 1);

                var p = Vector3.Lerp(p1, p2, t);
                var pNext = Vector3.Lerp(p1, p2, tNext);

                if (i != r - 1) {
                    Gizmos.DrawLine(p, pNext); // line
                }

                Gizmos.color = Color.white;
                Gizmos.DrawLine(p - deltaX, p + deltaX);
                Gizmos.DrawLine(p - deltaZ, p + deltaZ);

            }
        }

        public static void DebugDrawSphere(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 radius, Color color) {
            Gizmos.color = color;
            var start = position;

            var xoffs = new Vector3(radius.x * scale.x, 0, 0);
            var yoffs = new Vector3(0, radius.y * scale.y, 0);
            var zoffs = new Vector3(0, 0, radius.z * scale.z);

            xoffs = rotation * xoffs;
            yoffs = rotation * yoffs;
            zoffs = rotation * zoffs;

            var step = 5 * RADS_PER_DEG;
            var nSteps = (int)(360.0f / step);

            var vx = new Vector3(scale.x, 0, 0);
            var vy = new Vector3(0, scale.y, 0);
            var vz = new Vector3(0, 0, scale.z);

            var prev = start - xoffs;

            for (var i = 1; i <= nSteps; i++) {
                var angle = 360.0f * i / nSteps;
                var next = start + rotation * (radius.x * vx * Mathf.Cos(angle) + radius.y * vy * Mathf.Sin(angle));
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            prev = start - xoffs;
            for (var i = 1; i <= nSteps; i++) {
                var angle = 360.0f * i / nSteps;
                var next = start + rotation * (radius.x * vx * Mathf.Cos(angle) + radius.z * vz * Mathf.Sin(angle));
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            prev = start - yoffs;
            for (var i = 1; i <= nSteps; i++) {
                var angle = 360.0f * i / nSteps;
                var next = start + rotation * (radius.y * vy * Mathf.Cos(angle) + radius.z * vz * Mathf.Sin(angle));
                Gizmos.DrawLine(prev, next);
                prev = next;
            }


        }

        public static void DebugDrawPatch(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 c00, Vector3 c01, Vector3 c10, Vector3 c11, int resX, int resY, Color color) {
            if (resX < 2 || resY < 2)
                return;

            var matrix = Matrix4x4.TRS(position, rotation, scale);
            Gizmos.color = color;

            var p00 = matrix.MultiplyPoint(c00);
            var p01 = matrix.MultiplyPoint(c01);
            var p10 = matrix.MultiplyPoint(c10);
            var p11 = matrix.MultiplyPoint(c11);

            for (var iy = 0; iy < resY; ++iy) {
                for (var ix = 0; ix < resX; ++ix) {
                    // point 00
                    var tx_00 = ix * 1.0f / (resX - 1);
                    var ty_00 = iy * 1.0f / (resY - 1);

                    var py0_00 = Vector3.Lerp(p00, p01, ty_00);
                    var py1_00 = Vector3.Lerp(p10, p11, ty_00);
                    var pxy_00 = Vector3.Lerp(py0_00, py1_00, tx_00);

                    // point 01
                    var tx_01 = (ix + 1) * 1.0f / (resX - 1);
                    var ty_01 = iy * 1.0f / (resY - 1);

                    var py0_01 = Vector3.Lerp(p00, p01, ty_01);
                    var py1_01 = Vector3.Lerp(p10, p11, ty_01);
                    var pxy_01 = Vector3.Lerp(py0_01, py1_01, tx_01);

                    //point 10
                    var tx_10 = ix * 1.0f / (resX - 1);
                    var ty_10 = (iy + 1) * 1.0f / (resY - 1);

                    var py0_10 = Vector3.Lerp(p00, p01, ty_10);
                    var py1_10 = Vector3.Lerp(p10, p11, ty_10);
                    var pxy_10 = Vector3.Lerp(py0_10, py1_10, tx_10);

                    //point 11
                    var tx_11 = (ix + 1) * 1.0f / (resX - 1);
                    var ty_11 = (iy + 1) * 1.0f / (resY - 1);

                    var py0_11 = Vector3.Lerp(p00, p01, ty_11);
                    var py1_11 = Vector3.Lerp(p10, p11, ty_11);
                    var pxy_11 = Vector3.Lerp(py0_11, py1_11, tx_11);

                    Gizmos.DrawLine(pxy_00, pxy_01);
                    Gizmos.DrawLine(pxy_01, pxy_11);
                    Gizmos.DrawLine(pxy_00, pxy_11);
                    Gizmos.DrawLine(pxy_00, pxy_10);
                    Gizmos.DrawLine(pxy_10, pxy_11);
                }
            }
        }

        public static void DebugDrawBox(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 maxVec, Color color) {
            var minVec = new Vector3(0 - maxVec.x, 0 - maxVec.y, 0 - maxVec.z);

            var matrix = Matrix4x4.TRS(position, rotation, scale);
            var iii = matrix.MultiplyPoint(minVec);
            var aii = matrix.MultiplyPoint(new Vector3(maxVec[0], minVec[1], minVec[2]));
            var aai = matrix.MultiplyPoint(new Vector3(maxVec[0], maxVec[1], minVec[2]));
            var iai = matrix.MultiplyPoint(new Vector3(minVec[0], maxVec[1], minVec[2]));
            var iia = matrix.MultiplyPoint(new Vector3(minVec[0], minVec[1], maxVec[2]));
            var aia = matrix.MultiplyPoint(new Vector3(maxVec[0], minVec[1], maxVec[2]));
            var aaa = matrix.MultiplyPoint(maxVec);
            var iaa = matrix.MultiplyPoint(new Vector3(minVec[0], maxVec[1], maxVec[2]));

            Gizmos.color = color;

            Gizmos.DrawLine(iii, aii);
            Gizmos.DrawLine(aii, aai);
            Gizmos.DrawLine(aai, iai);
            Gizmos.DrawLine(iai, iii);
            Gizmos.DrawLine(iii, iia);
            Gizmos.DrawLine(aii, aia);
            Gizmos.DrawLine(aai, aaa);
            Gizmos.DrawLine(iai, iaa);
            Gizmos.DrawLine(iia, aia);
            Gizmos.DrawLine(aia, aaa);
            Gizmos.DrawLine(aaa, iaa);
            Gizmos.DrawLine(iaa, iia);
        }

        public static void DebugDrawCapsule(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float halfHeight, int upAxis, Color color) {

            var matrix = Matrix4x4.TRS(position, rotation, scale);

            Gizmos.color = color;

            var capStart = new Vector3(0.0f, 0.0f, 0.0f);
            capStart[upAxis] = -halfHeight;

            var capEnd = new Vector3(0.0f, 0.0f, 0.0f);
            capEnd[upAxis] = halfHeight;

            Gizmos.DrawWireSphere(matrix.MultiplyPoint(capStart), radius);
            Gizmos.DrawWireSphere(matrix.MultiplyPoint(capEnd), radius);

            // Draw some additional lines
            var start = position;

            capStart[(upAxis + 1) % 3] = radius;
            capEnd[(upAxis + 1) % 3] = radius;
            Gizmos.DrawLine(start + rotation * capStart, start + rotation * capEnd);

            capStart[(upAxis + 1) % 3] = -radius;
            capEnd[(upAxis + 1) % 3] = -radius;
            Gizmos.DrawLine(start + rotation * capStart, start + rotation * capEnd);

            capStart[(upAxis + 1) % 3] = 0.0f;
            capEnd[(upAxis + 1) % 3] = 0.0f;

            capStart[(upAxis + 2) % 3] = radius;
            capEnd[(upAxis + 2) % 3] = radius;
            Gizmos.DrawLine(start + rotation * capStart, start + rotation * capEnd);

            capStart[(upAxis + 2) % 3] = -radius;
            capEnd[(upAxis + 2) % 3] = -radius;
            Gizmos.DrawLine(start + rotation * capStart, start + rotation * capEnd);

        }

        public static void DebugDrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float halfHeight, int upAxis, Color color) {
            Gizmos.color = color;
            var start = position;
            var offsetHeight = new Vector3(0, 0, 0);
            offsetHeight[upAxis] = halfHeight;
            var offsetRadius = new Vector3(0, 0, 0);
            offsetRadius[(upAxis + 1) % 3] = radius;

            offsetHeight.x *= scale.x; offsetHeight.y *= scale.y; offsetHeight.z *= scale.z;
            offsetRadius.x *= scale.x; offsetRadius.y *= scale.y; offsetRadius.z *= scale.z;

            Gizmos.DrawLine(start + rotation * (offsetHeight + offsetRadius), start + rotation * (-offsetHeight + offsetRadius));
            Gizmos.DrawLine(start + rotation * (offsetHeight - offsetRadius), start + rotation * (-offsetHeight - offsetRadius));

            // Drawing top and bottom caps of the cylinder
            var yaxis = new Vector3(0, 0, 0);
            yaxis[upAxis] = 1.0f;
            var xaxis = new Vector3(0, 0, 0);
            xaxis[(upAxis + 1) % 3] = 1.0f;

            var r = offsetRadius.magnitude;
            DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, Two_PI, color, false, 10.0f);
            DebugDrawArc(start + rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, Two_PI, color, false, 10.0f);
        }

        public static void DebugDrawCone(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float height, int upAxis, Color color) {
            Gizmos.color = color;

            var start = position;

            var offsetHeight = new Vector3(0, 0, 0);
            offsetHeight[upAxis] = height * 0.5f;
            var offsetRadius = new Vector3(0, 0, 0);
            offsetRadius[(upAxis + 1) % 3] = radius;
            var offset2Radius = new Vector3(0, 0, 0);
            offset2Radius[(upAxis + 2) % 3] = radius;

            offsetHeight.x *= scale.x; offsetHeight.y *= scale.y; offsetHeight.z *= scale.z;
            offsetRadius.x *= scale.x; offsetRadius.y *= scale.y; offsetRadius.z *= scale.z;
            offset2Radius.x *= scale.x; offset2Radius.y *= scale.y; offset2Radius.z *= scale.z;

            Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight + offsetRadius));
            Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight - offsetRadius));
            Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight + offset2Radius));
            Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight - offset2Radius));

            // Drawing the base of the cone
            var yaxis = new Vector3(0, 0, 0);
            yaxis[upAxis] = 1.0f;
            var xaxis = new Vector3(0, 0, 0);
            xaxis[(upAxis + 1) % 3] = 1.0f;
            DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, offsetRadius.magnitude, offset2Radius.magnitude, 0, Two_PI, color, false, 10.0f);
        }

        public static void DebugDrawPlane(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 planeNormal, float planeConst, Color color) {
            var matrix = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1));


            Gizmos.color = color;

            var planeOrigin = planeNormal * planeConst;
            var vec0 = new Vector3(0, 0, 0);
            var vec1 = new Vector3(0, 0, 0);
            GetPlaneSpaceVector(planeNormal, ref vec0, ref vec1);
            var vecLen = 100.0f;
            var pt0 = planeOrigin + vec0 * vecLen;
            var pt1 = planeOrigin - vec0 * vecLen;
            var pt2 = planeOrigin + vec1 * vecLen;
            var pt3 = planeOrigin - vec1 * vecLen;
            Gizmos.DrawLine(matrix.MultiplyPoint(pt0), matrix.MultiplyPoint(pt1));
            Gizmos.DrawLine(matrix.MultiplyPoint(pt2), matrix.MultiplyPoint(pt3));

        }

        public static void GetPlaneSpaceVector(Vector3 planeNormal, ref Vector3 vec1, ref Vector3 vec2) {
            if (Mathf.Abs(planeNormal[2]) > SQRT12) {
                // choose p in y-z plane
                var a = planeNormal[1] * planeNormal[1] + planeNormal[2] * planeNormal[2];
                var k = 1.0f / Mathf.Sqrt(a);
                vec1[0] = 0;
                vec1[1] = -planeNormal[2] * k;
                vec1[2] = planeNormal[1] * k;
                // set q = n x p
                vec2[0] = a * k;
                vec2[1] = -planeNormal[0] * vec1[2];
                vec2[2] = planeNormal[0] * vec1[1];
            } else {
                // choose p in x-y plane
                var a = planeNormal[0] * planeNormal[0] + planeNormal[1] * planeNormal[1];
                var k = 1.0f / Mathf.Sqrt(a);
                vec1[0] = -planeNormal[1] * k;
                vec1[1] = planeNormal[0] * k;
                vec1[2] = 0;
                // set q = n x p
                vec2[0] = -planeNormal[2] * vec1[1];
                vec2[1] = planeNormal[2] * vec1[0];
                vec2[2] = a * k;
            }
        }

        public static void DebugDrawArc(Vector3 center, Vector3 normal, Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle,
                    Color color, bool drawSect, float stepDegrees) {
            Gizmos.color = color;

            var vx = axis;
            var vy = Vector3.Cross(normal, axis);
            var step = stepDegrees * RADS_PER_DEG;
            var nSteps = (int)((maxAngle - minAngle) / step);
            if (nSteps == 0)
                nSteps = 1;
            var prev = center + radiusA * vx * Mathf.Cos(minAngle) + radiusB * vy * Mathf.Sin(minAngle);
            if (drawSect) {
                Gizmos.DrawLine(center, prev);
            }
            for (var i = 1; i <= nSteps; i++) {
                var angle = minAngle + (maxAngle - minAngle) * i * 1.0f / (nSteps * 1.0f);
                var next = center + radiusA * vx * Mathf.Cos(angle) + radiusB * vy * Mathf.Sin(angle);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            if (drawSect) {
                Gizmos.DrawLine(center, prev);
            }
        }
    }
}
