using UnityEngine;

namespace CozySpringJam.Utils
{
    public static class Extensions
    {
        private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        public static Vector3 ToIsometric(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

        public static void CopyPropertiesToMaterial(this Material source, Material destination)
        {
            destination.CopyPropertiesFromMaterial(source);
        }
    }
}
