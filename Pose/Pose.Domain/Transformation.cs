using System;
using Pose.Common;

namespace Pose.Domain
{
    public class Transformation
    {
        private readonly Transformation _parentTransformation;

        public readonly Vector2 LocalTranslation;
        public readonly float LocalRotation;
        public readonly Vector2 LocalScale;
        
        public readonly Vector2 GlobalTranslation;
        public readonly Vector2 GlobalScale;
        public readonly float GlobalRotation;
        public readonly Matrix GlobalTransform;

        public Transformation(Vector2 localTranslation, float localRotation, Vector2 localScale, Transformation parentTransformation)
        {
            LocalTranslation = localTranslation;
            LocalRotation = localRotation;
            LocalScale = localScale;
            _parentTransformation = parentTransformation;

            GlobalTransform = CalcGlobalMatrix(parentTransformation);
            GlobalTranslation = new Vector2(GlobalTransform.M13, GlobalTransform.M23); // global translation can be reversed from the matrix.
            GlobalRotation = parentTransformation?.GlobalRotation + localRotation ?? localRotation; // global rotation cannot be reverse from matrix, but is just adding rotations of parents
            GlobalScale = parentTransformation?.GlobalScale + localScale ?? localScale; // like rotation
        }

        private Matrix CalcGlobalMatrix(Transformation parentTransformation)
        {
            var sin = MathF.Sin(LocalRotation);
            var cos = MathF.Cos(LocalRotation);

            var localTransform = new Matrix(
                cos * LocalScale.X, -sin, LocalTranslation.X,
                sin, cos * LocalScale.Y, LocalTranslation.Y,
                0f, 0f, 1f);
            if (parentTransformation == null)
                return localTransform;

            return parentTransformation.GlobalTransform * localTransform;
        }

        public Transformation WithLocalTranslation(in Vector2 translation)
        {
            return new Transformation(translation, LocalRotation, LocalScale, _parentTransformation);
        }

        public Transformation WithLocalRotation(in float angle)
        {
            return new Transformation(LocalTranslation, angle, LocalScale, _parentTransformation);
        }
    }
}
