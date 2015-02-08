﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoidEngine
{
    public class Collision
    {
        public struct MapSegment
        {
            public Point point1;
            public Point point2;

            public MapSegment(Point a, Point b)
            {
                point1 = a;
                point2 = b;
            }

            public Vector2 getVector()
            {
                return new Vector2(point2.X - point1.X, point2.Y - point1.Y);
            }

            public Rectangle collisionRect()
            {
                return new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
            }
		}

		public struct Line2D
		{
			public Vector2 point;
			public Vector2 vector;

			public float yInt()
			{
				return (-vector.Y * point.X + vector.X * point.Y) / vector.X;
			}
			public float Slope()
			{
				return vector.Y / vector.X;
			}
		}

		public struct Circle
		{
			public Vector2 point;
			public double radius;

			public Circle(Vector2 point, double radius)
			{
				this.point = point;
				this.radius = radius;
			}
		}

        public static float Magnitude(Vector2 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static Vector2 VectorNormal(Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 UnitVectorS(Vector2 vector)
        {
            return new Vector2(vector.X / (float)Magnitude(vector), vector.Y / (float)Magnitude(vector));
        }

        public static float DotProduct(Vector2 unitVector, Vector2 vector)
        {
            return unitVector.X * vector.X + unitVector.Y * vector.Y;
        }

        public static Vector2 reflectedVector(Vector2 vector, Vector2 reflectVector)
        {
            Vector2 normal = VectorNormal(reflectVector);
            float coeficient = -2 * (DotProduct(vector, normal) / (Magnitude(normal) * Magnitude(normal)));
            Vector2 r;
            r.X = vector.X + coeficient * normal.X;
            r.Y = vector.Y + coeficient * normal.Y;
            return r;
        }

        public static bool CheckSegmentSegmentCollision(MapSegment segemnt1, MapSegment segment2)
        {
            Line2D L1, L2;
            L1.point = new Vector2(segemnt1.point1.X, segemnt1.point1.Y);
            L2.point = new Vector2(segment2.point1.X, segment2.point1.Y);
            L1.vector.X = segemnt1.point2.X - segemnt1.point1.X;
            L1.vector.Y = segemnt1.point2.Y - segemnt1.point1.X;
            L2.vector.X = segment2.point2.X - segment2.point1.X;
            L2.vector.Y = segment2.point2.Y - segment2.point1.Y;
            Vector2 CollisionPoint;
            CollisionPoint.X = (L2.yInt() - L1.yInt()) / (L1.Slope() - L2.Slope());
            CollisionPoint.Y = L1.Slope() * CollisionPoint.X + L1.yInt();

            bool cond1 = (Math.Min(segemnt1.point1.X, segemnt1.point1.X) <= CollisionPoint.X && CollisionPoint.X <= Math.Max(segment2.point1.X, segemnt1.point2.X));
            bool cond2 = (Math.Min(segment2.point1.X, segment2.point2.X) <= CollisionPoint.X && CollisionPoint.X <= Math.Max(segment2.point1.X, segment2.point2.X));
            bool cond3 = (Math.Min(segemnt1.point1.Y, segemnt1.point2.Y) <= CollisionPoint.Y && CollisionPoint.Y <= Math.Max(segemnt1.point1.Y, segemnt1.point2.Y));
            bool cond4 = (Math.Min(segment2.point1.Y, segment2.point2.Y) <= CollisionPoint.Y && CollisionPoint.Y <= Math.Max(segment2.point1.Y, segment2.point2.Y));

            return cond1 && cond2 && cond3 && cond4;
        }

        public static bool CheckCircleSegmentCollision(Circle Circle, MapSegment Segement)
        {
            Line2D Line;
            Line.point.X = Segement.point1.X;
            Line.point.Y = Segement.point1.Y;
            Line.vector.X = Segement.point2.X - Segement.point1.X;
            Line.vector.Y = Segement.point2.Y - Segement.point1.Y;


            double OH = Math.Abs(((Line.vector.X * (Circle.point.Y)) - (Line.vector.Y * (Circle.point.X - Line.point.X))) / (Math.Sqrt(Line.vector.X * Line.vector.X + Line.vector.Y * Line.vector.Y)));

            if (OH <= Circle.radius)
            {
                Vector2 CollisionPoint1;
                Vector2 CollisionPoint2;

                if (Line.vector.X != 0)
                {
                    double Dv = Line.vector.Y / Line.vector.X;
                    double E = (Line.vector.X * Line.point.Y - Line.vector.Y * Line.point.X) / Line.vector.X - Circle.point.Y;

                    double a = 1 + Dv * Dv;
                    double b = -2 * Circle.point.X + 2 * E * Dv;
                    double c = Circle.point.X * Circle.point.X + E * E - Circle.radius * Circle.radius;

                    CollisionPoint1.X = (float)((-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a));
                    CollisionPoint2.X = (float)((-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a));

                    CollisionPoint1.Y = Line.Slope() * CollisionPoint1.X + Line.yInt();
                    CollisionPoint2.Y = Line.Slope() * CollisionPoint1.X + Line.yInt();

                    bool cond1 = (Math.Min(Segement.point1.X, Segement.point2.X) <= CollisionPoint1.X && CollisionPoint1.X <= Math.Max(Segement.point1.X, Segement.point2.X));

                    bool cond2 = (Math.Min(Segement.point1.Y, Segement.point2.Y) <= CollisionPoint1.Y && CollisionPoint1.Y <= Math.Max(Segement.point1.Y, Segement.point2.Y));

                    bool cond3 = (Math.Min(Segement.point1.X, Segement.point2.X) <= CollisionPoint2.X && CollisionPoint2.X <= Math.Max(Segement.point1.X, Segement.point2.X));

                    bool cond4 = (Math.Min(Segement.point1.Y, Segement.point2.Y) <= CollisionPoint2.Y && CollisionPoint2.Y <= Math.Max(Segement.point1.Y, Segement.point2.Y));

                    return (cond1 && cond2) || (cond3 && cond4);
                }
            }

            return false;
        }

		public static bool CheckCircleCircleCollision(Circle circle1, Circle circle2)
		{
			return (circle1.radius + circle2.radius >= Magnitude(circle2.point - circle1.point));
		}
    }
}