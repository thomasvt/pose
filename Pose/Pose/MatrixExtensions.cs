using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Pose
{
    public static class MatrixExtensions
    {
        public static Matrix ToWpfMatrix(this Common.Matrix matrix)
        {
            // convert by transposing... we use transposed matrices, I didnt know most software stores it like that..
            return new Matrix(matrix.M11, matrix.M21, matrix.M12, matrix.M22, matrix.M13, matrix.M23);
        }

        public static Matrix3D ToWpfMatrix3D(this Common.Matrix matrix)
        {
            // this is probably partly wrong, its only tested for Z rotation and translation of X,Y
            return new Matrix3D(matrix.M11, matrix.M21, matrix.M31, 0, 
                matrix.M12, matrix.M22, matrix.M32, 0f,
                0,0,1,0,
                matrix.M13, matrix.M23, 0f, 1);

        }
    }
}
