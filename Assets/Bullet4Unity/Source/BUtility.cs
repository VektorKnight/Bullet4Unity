using UnityEngine;

namespace Bullet4Unity {
    /// <summary>
    /// Re-implemented from the BulletSharpUnity3d project
    /// Portions of code credit to Phong13
    /// </summary>
    public class BUtility {
        public const float TWO_PI = 6.283185307179586232f;
        public const float RADS_PER_DEG = TWO_PI / 360.0f;
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
                    var tx00 = ix * 1.0f / (resX - 1);
                    var ty00 = iy * 1.0f / (resY - 1);

                    var py000 = Vector3.Lerp(p00, p01, ty00);
                    var py100 = Vector3.Lerp(p10, p11, ty00);
                    var pxy00 = Vector3.Lerp(py000, py100, tx00);

                    // point 01
                    var tx01 = (ix + 1) * 1.0f / (resX - 1);
                    var ty01 = iy * 1.0f / (resY - 1);

                    var py001 = Vector3.Lerp(p00, p01, ty01);
                    var py101 = Vector3.Lerp(p10, p11, ty01);
                    var pxy01 = Vector3.Lerp(py001, py101, tx01);

                    //point 10
                    var tx10 = ix * 1.0f / (resX - 1);
                    var ty10 = (iy + 1) * 1.0f / (resY - 1);

                    var py010 = Vector3.Lerp(p00, p01, ty10);
                    var py110 = Vector3.Lerp(p10, p11, ty10);
                    var pxy10 = Vector3.Lerp(py010, py110, tx10);

                    //point 11
                    var tx11 = (ix + 1) * 1.0f / (resX - 1);
                    var ty11 = (iy + 1) * 1.0f / (resY - 1);

                    var py011 = Vector3.Lerp(p00, p01, ty11);
                    var py111 = Vector3.Lerp(p10, p11, ty11);
                    var pxy11 = Vector3.Lerp(py011, py111, tx11);

                    Gizmos.DrawLine(pxy00, pxy01);
                    Gizmos.DrawLine(pxy01, pxy11);
                    Gizmos.DrawLine(pxy00, pxy11);
                    Gizmos.DrawLine(pxy00, pxy10);
                    Gizmos.DrawLine(pxy10, pxy11);
                }
            }
        }


        /*	
            //it is very slow, so don't use it if you don't need it indeed..
            public static void DebugDrawPolyhedron(Vector3 position,Quaternion rotation,Vector3 scale,btPolyhedralConvexShape shape,Color color)
            {
                if( shape == null )
                    return;

                Matrix4x4 matrix = Matrix4x4.TRS(position,rotation,scale);
                Gizmos.color = color;
                btConvexPolyhedron poly = shape.getConvexPolyhedron();
                if( poly == null )
                    return;

                int faceSize = poly.m_faces.size();
                for (int i=0;i < faceSize;i++)
                {
                    Vector3 centroid = new Vector3(0,0,0);
                    btFace face = poly.m_faces.at(i);
                    int numVerts = face.m_indices.size();
                    if (numVerts > 0)
                    {
                        int lastV = face.m_indices.at(numVerts-1);
                        for (int v=0;v < numVerts;v++)
                        {
                            int curVert = face.m_indices.at(v);
                            BulletSharp.Math.Vector3 curVertObject = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(curVert));
                            centroid.x += curVertObject.x();
                            centroid.y += curVertObject.y();
                            centroid.z += curVertObject.z();
                            BulletSharp.Math.Vector3 btv1 = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(lastV));
                            BulletSharp.Math.Vector3 btv2 = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(curVert));
                            Vector3 v1 = new Vector3(btv1.x(),btv1.y(),btv1.z());
                            Vector3 v2 = new Vector3(btv2.x(),btv2.y(),btv2.z());
                            v1 = matrix.MultiplyPoint(v1);
                            v2 = matrix.MultiplyPoint(v2);
                            Gizmos.DrawLine(v1,v2);
                            lastV = curVert;
                        }
                    }
                    float s = 1.0f/numVerts;
                    centroid.x *= s;
                    centroid.y *= s;
                    centroid.z *= s;

                    //normal draw
        //            {
        //                Vector3 normalColor = new Vector3(1,1,0);
        //				 
        //                BulletSharp.Math.Vector3 faceNormal(face.m_plane[0],poly->m_faces[i].m_plane[1],poly->m_faces[i].m_plane[2]);
        //                getDebugDrawer()->drawLine(worldTransform*centroid,worldTransform*(centroid+faceNormal),normalColor);
        //            }

                }
            }
        */
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
            var offsetHeight = new Vector3(0, 0, 0) {[upAxis] = halfHeight};
            var offsetRadius = new Vector3(0, 0, 0) {[(upAxis + 1) % 3] = radius};

            offsetHeight.x *= scale.x; offsetHeight.y *= scale.y; offsetHeight.z *= scale.z;
            offsetRadius.x *= scale.x; offsetRadius.y *= scale.y; offsetRadius.z *= scale.z;

            Gizmos.DrawLine(start + rotation * (offsetHeight + offsetRadius), start + rotation * (-offsetHeight + offsetRadius));
            Gizmos.DrawLine(start + rotation * (offsetHeight - offsetRadius), start + rotation * (-offsetHeight - offsetRadius));

            // Drawing top and bottom caps of the cylinder
            var yaxis = new Vector3(0, 0, 0) {[upAxis] = 1.0f};
            var xaxis = new Vector3(0, 0, 0) {[(upAxis + 1) % 3] = 1.0f};

            var r = offsetRadius.magnitude;
            DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, TWO_PI, color, false, 10.0f);
            DebugDrawArc(start + rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, TWO_PI, color, false, 10.0f);
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
            DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, offsetRadius.magnitude, offset2Radius.magnitude, 0, TWO_PI, color, false, 10.0f);
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
