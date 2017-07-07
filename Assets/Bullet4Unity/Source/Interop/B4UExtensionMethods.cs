using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Reflection;

namespace Bullet4Unity {
    /// <summary>
    /// Extension methods for converting common data types between BulletSharp and Unity
    /// - Reimplemented from BulletSharpUnity3D
    /// </summary>
    public static class B4UExtensionMethods {
        
        /// <summary>
        /// Convert a Unity Quaternion to BulletSharp
        /// </summary>
        /// <param name="q">Quaternion to be converted</param>
        /// <returns></returns>
        public static BulletSharp.Math.Quaternion ToBullet(this UnityEngine.Quaternion q) {
            return new BulletSharp.Math.Quaternion(q.x, q.y, q.z, q.w);
        }
        
        /// <summary>
        /// Convert a BulletSharp Quaternion to Unity
        /// </summary>
        /// <param name="q">Quaternion to be converted</param>
        /// <returns></returns>
        public static UnityEngine.Quaternion ToUnity(this BulletSharp.Math.Quaternion q) {
            return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
        }
        
        /// <summary>
        /// Convert a Unity Vector3 to BulletSharp
        /// </summary>
        /// <param name="v">Vector3 to be converted</param>
        /// <returns></returns>
        public static BulletSharp.Math.Vector3 ToBullet(this UnityEngine.Vector3 v) {
            return new BulletSharp.Math.Vector3(v.x, v.y, v.z);
        }
    
        /// <summary>
        /// Convert a BulletSharp Vector3 to Unity
        /// </summary>
        /// <param name="v">Vector3 to be converted</param>
        /// <returns></returns>
        public static UnityEngine.Vector3 ToUnity(this BulletSharp.Math.Vector3 v) {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Convert a BulletSharp Matrix4x4 to Unity
        /// </summary>
        /// <param name="bMatrix4x4">Matrix to be converted</param>
        /// <returns></returns>
        public static UnityEngine.Matrix4x4 ToUnity(this BulletSharp.Math.Matrix bMatrix4x4) {
            var uMatrix4X4 = new Matrix4x4();
            uMatrix4X4[0, 0] = bMatrix4x4[0, 0];
            uMatrix4X4[0, 1] = bMatrix4x4[1, 0];
            uMatrix4X4[0, 2] = bMatrix4x4[2, 0];
            uMatrix4X4[0, 3] = bMatrix4x4[3, 0];

            uMatrix4X4[1, 0] = bMatrix4x4[0, 1];
            uMatrix4X4[1, 1] = bMatrix4x4[1, 1];
            uMatrix4X4[1, 2] = bMatrix4x4[2, 1];
            uMatrix4X4[1, 3] = bMatrix4x4[3, 1];

            uMatrix4X4[2, 0] = bMatrix4x4[0, 2];
            uMatrix4X4[2, 1] = bMatrix4x4[1, 2];
            uMatrix4X4[2, 2] = bMatrix4x4[2, 2];
            uMatrix4X4[2, 3] = bMatrix4x4[3, 2];

            uMatrix4X4[3, 0] = bMatrix4x4[0, 3];
            uMatrix4X4[3, 1] = bMatrix4x4[1, 3];
            uMatrix4X4[3, 2] = bMatrix4x4[2, 3];
            uMatrix4X4[3, 3] = bMatrix4x4[3, 3];
            return uMatrix4X4;
        }

        /// <summary>
        /// Get Quaternion rotation from BulletSharp Matrix4x4
        /// </summary>
        /// <param name="bMatrix4x4">Matrix from which to extract a rotatio</param>
        /// <returns></returns>
        public static BulletSharp.Math.Quaternion GetOrientation(this BulletSharp.Math.Matrix bMatrix4x4) {
                //Scaling is the length of the rows.
                BulletSharp.Math.Vector3 scale;
                scale.X = (float)System.Math.Sqrt((bMatrix4x4.M11 * bMatrix4x4.M11) + (bMatrix4x4.M12 * bMatrix4x4.M12) + (bMatrix4x4.M13 * bMatrix4x4.M13));
                scale.Y = (float)System.Math.Sqrt((bMatrix4x4.M21 * bMatrix4x4.M21) + (bMatrix4x4.M22 * bMatrix4x4.M22) + (bMatrix4x4.M23 * bMatrix4x4.M23));
                scale.Z = (float)System.Math.Sqrt((bMatrix4x4.M31 * bMatrix4x4.M31) + (bMatrix4x4.M32 * bMatrix4x4.M32) + (bMatrix4x4.M33 * bMatrix4x4.M33));

                //Divide out scaling to get rotation
                var mm11 = bMatrix4x4.M11 / scale.X;
                var mm12 = bMatrix4x4.M12 / scale.X;
                var mm13 = bMatrix4x4.M13 / scale.X;

                var mm21 = bMatrix4x4.M21 / scale.Y;
                var mm22 = bMatrix4x4.M22 / scale.Y;
                var mm23 = bMatrix4x4.M23 / scale.Y;

                var mm31 = bMatrix4x4.M31 / scale.Z;
                var mm32 = bMatrix4x4.M32 / scale.Z;
                var mm33 = bMatrix4x4.M33 / scale.Z;


                float sqrt;
                float half;
                var trace = mm11 + mm22 + mm33;
                var result = new BulletSharp.Math.Quaternion();
                if (trace > 0.0f){
                    sqrt = (float)UnityEngine.Mathf.Sqrt(trace + 1.0f);
                    result.W = sqrt * 0.5f;
                    sqrt = 0.5f / sqrt;

                    result.X = (mm23 - mm32) * sqrt;
                    result.Y = (mm31 - mm13) * sqrt;
                    result.Z = (mm12 - mm21) * sqrt;
                }
                else if ((mm11 >= mm22) && (mm11 >= mm33)) {
                    sqrt = (float)UnityEngine.Mathf.Sqrt(1.0f + mm11 - mm22 - mm33);
                    half = 0.5f / sqrt;

                    result.X = 0.5f * sqrt;
                    result.Y = (mm12 + mm21) * half;
                    result.Z = (mm13 + mm31) * half;
                    result.W = (mm23 - mm32) * half;
                }
                else if (mm22 > mm33) {
                    sqrt = (float)UnityEngine.Mathf.Sqrt(1.0f + mm22 - mm11 - mm33);
                    half = 0.5f / sqrt;

                    result.X = (mm21 + mm12) * half;
                    result.Y = 0.5f * sqrt;
                    result.Z = (mm32 + mm23) * half;
                    result.W = (mm31 - mm13) * half;
                }
                else {
                    sqrt = (float)UnityEngine.Mathf.Sqrt(1.0f + mm33 - mm11 - mm22);
                    half = 0.5f / sqrt;

                    result.X = (mm31 + mm13) * half;
                    result.Y = (mm32 + mm23) * half;
                    result.Z = 0.5f * sqrt;
                    result.W = (mm12 - mm21) * half;
                }
                return result;
        }
        
        public static void SetOrientation(this BulletSharp.Math.Matrix bm, BulletSharp.Math.Quaternion q) {
                var xx = q.X * q.X;
                var yy = q.Y * q.Y;
                var zz = q.Z * q.Z;
                var xy = q.X * q.Y;
                var zw = q.Z * q.W;
                var zx = q.Z * q.X;
                var yw = q.Y * q.W;
                var yz = q.Y * q.Z;
                var xw = q.X * q.W;

                bm.M11 = 1.0f - (2.0f * (yy + zz));
                bm.M12 = 2.0f * (xy + zw);
                bm.M13 = 2.0f * (zx - yw);
                bm.M21 = 2.0f * (xy - zw);
                bm.M22 = 1.0f - (2.0f * (zz + xx));
                bm.M23 = 2.0f * (yz + xw);
                bm.M31 = 2.0f * (zx + yw);
                bm.M32 = 2.0f * (yz - xw);
                bm.M33 = 1.0f - (2.0f * (yy + xx));
        }

        public static BulletSharp.Math.Matrix ToBullet(this UnityEngine.Matrix4x4 um) {
            var bm = new BulletSharp.Math.Matrix();
            um.ToBullet(ref bm);
            return bm;
        }

        public static void ToBullet(this UnityEngine.Matrix4x4 um, ref BulletSharp.Math.Matrix bm) {
            bm[0, 0] = um[0, 0];
            bm[0, 1] = um[1, 0];
            bm[0, 2] = um[2, 0];
            bm[0, 3] = um[3, 0];

            bm[1, 0] = um[0, 1];
            bm[1, 1] = um[1, 1];
            bm[1, 2] = um[2, 1];
            bm[1, 3] = um[3, 1];

            bm[2, 0] = um[0, 2];
            bm[2, 1] = um[1, 2];
            bm[2, 2] = um[2, 2];
            bm[2, 3] = um[3, 2];

            bm[3, 0] = um[0, 3];
            bm[3, 1] = um[1, 3];
            bm[3, 2] = um[2, 3];
            bm[3, 3] = um[3, 3];
        }

        public static void SetTransformationFromBulletMatrix(this UnityEngine.Transform transform, BulletSharp.Math.Matrix bm) {
            var matrix = bm.ToUnity();  //creates new Unity Matrix4x4
            transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
            transform.localRotation = ExtractRotationFromMatrix(ref matrix);
            transform.localScale = ExtractScaleFromMatrix(ref matrix);
        }

        /// <summary>
        /// Extract translation from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Translation offset.
        /// </returns>
        public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
            Vector3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        public static Vector3 ExtractTranslationFromMatrix(ref BulletSharp.Math.Matrix matrix) {
            Vector3 translate;
            translate.x = matrix.M41;
            translate.y = matrix.M42;
            translate.z = matrix.M43;
            return translate;
        }

        /// <summary>
        /// Extract rotation quaternion from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Quaternion representation of rotation transform.
        /// </returns>
        public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Quaternion ExtractRotationFromMatrix(ref BulletSharp.Math.Matrix matrix) {
            Vector3 forward;
            forward.x = matrix.M31;
            forward.y = matrix.M32;
            forward.z = matrix.M33;

            Vector3 upwards;
            upwards.x = matrix.M21;
            upwards.y = matrix.M22;
            upwards.z = matrix.M23;

            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Extract scale from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Scale vector.
        /// </returns>
        public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        public static Vector3 ExtractScaleFromMatrix(ref BulletSharp.Math.Matrix matrix) {
            Vector3 scale;
            scale.x = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14).magnitude;
            scale.y = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24).magnitude;
            scale.z = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34).magnitude;
            return scale;
        }

        /// <summary>
        /// Extract position, rotation and scale from TRS matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <param name="localPosition">Output position.</param>
        /// <param name="localRotation">Output rotation.</param>
        /// <param name="localScale">Output scale.</param>
        public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
            localPosition = ExtractTranslationFromMatrix(ref matrix);
            localRotation = ExtractRotationFromMatrix(ref matrix);
            localScale = ExtractScaleFromMatrix(ref matrix);
        }

        /// <summary>
        /// Set transform component from TRS matrix.
        /// </summary>
        /// <param name="transform">Transform component.</param>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
            transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
            transform.localRotation = ExtractRotationFromMatrix(ref matrix);
            transform.localScale = ExtractScaleFromMatrix(ref matrix);
        }


        // EXTRAS!

        /// <summary>
        /// Identity quaternion.
        /// </summary>
        /// <remarks>
        /// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
        /// </remarks>
        public static readonly Quaternion IdentityQuaternion = Quaternion.identity;
        /// <summary>
        /// Identity matrix.
        /// </summary>
        /// <remarks>
        /// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
        /// </remarks>
        public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;

        /// <summary>
        /// Get translation matrix.
        /// </summary>
        /// <param name="offset">Translation offset.</param>
        /// <returns>
        /// The translation transform matrix.
        /// </returns>
        public static Matrix4x4 TranslationMatrix(Vector3 offset) {
            var matrix = IdentityMatrix;
            matrix.m03 = offset.x;
            matrix.m13 = offset.y;
            matrix.m23 = offset.z;
            return matrix;
        }
    }
}

