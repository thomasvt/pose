using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pose.Domain.Tests
{
    [TestClass]
    public class Matrix_Tests
    {
        [TestMethod]
        public void Determinant_Should_Be_74()
        {
            var sut = new Matrix(1, 5, 3, 2, 4, 7, 4, 6, 2);
            var det = sut.GetDeterminant();

            Assert.AreEqual(74, det);
        }

        [TestMethod]
        public void Inverse_Test1()
        {
            var sut = new Matrix(1, 2, 3, 0, 1, 4, 5, 6, 0);
            var inverse = sut.GetInverse();

            Assert.AreEqual(new Matrix(-24, 18, 5, 20, -15, -4, -5, 4, 1), inverse);
        }

        [TestMethod]
        public void Inverse_Test2()
        {
            var sut = new Matrix(3, 0, 2, 2, 0, -2, 0, 1, 1);
            var inverse = sut.GetInverse();

            Assert.AreEqual(new Matrix(0.2f, 0.2f, 0, -0.2f, 0.3f, 1, 0.2f, -0.3f, 0), inverse);
        }
    }
}
