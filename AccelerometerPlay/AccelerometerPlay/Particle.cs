/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

// Each of our particle holds its previous and current position, its
// acceleration.  For added realism each particle has its own friction
// coefficient.
namespace AccelerometerPlay
{
	class Particle
	{
		private ParticleSystem system;

		private PointF prev_location = new PointF ();
		private PointF accel = new PointF ();
		private float friction;

		public PointF Location { get; private set; }

		public Particle (ParticleSystem system)
		{
			this.system = system;
			Location = new PointF ();

			// Make each particle a bit different by randomizing its
			// coefficient of friction
			var random = new Random ();
			var r = ((float)random.NextDouble () - 0.5f) * 0.2f;

			friction = 1.0f - SimulationView.FRICTION + r;
		}

		public void ComputePhysics (float sx, float sy, float dT, float dTC)
		{
			// Force of gravity applied to our virtual object
			float m = 1000.0f; // mass of our virtual object
			float gx = -sx * m;
			float gy = -sy * m;

			// ·F = mA <=> A = ·F / m We could simplify the code by
			// completely eliminating "m" (the mass) from all the equations,
			// but it would hide the concepts from this sample code.
			float invm = 1.0f / m;
			float ax = gx * invm;
			float ay = gy * invm;

			// Time-corrected Verlet integration The position Verlet
			// integrator is defined as x(t+Æt) = x(t) + x(t) - x(t-Æt) +
			// a(t)Ætö2 However, the above equation doesn't handle variable
			// Æt very well, a time-corrected version is needed: x(t+Æt) =
			// x(t) + (x(t) - x(t-Æt)) * (Æt/Æt_prev) + a(t)Ætö2 We also add
			// a simple friction term (f) to the equation: x(t+Æt) = x(t) +
			// (1-f) * (x(t) - x(t-Æt)) * (Æt/Æt_prev) + a(t)Ætö2
			float dTdT = dT * dT;
			float x = Location.X + friction * dTC * (Location.X - prev_location.X) + accel.X * dTdT;
			float y = Location.Y + friction * dTC * (Location.Y - prev_location.Y) + accel.Y * dTdT;

			prev_location.Set (Location);
			Location.Set (x, y);
			accel.Set (ax, ay);
		}

		// Resolving constraints and collisions with the Verlet integrator
		// can be very simple, we simply need to move a colliding or
		// constrained particle in such way that the constraint is
		// satisfied.
		public void ResolveCollisionWithBounds ()
		{
			float xmax = system.sim_view.Bounds.X;
			float ymax = system.sim_view.Bounds.Y;
			float x = Location.X;
			float y = Location.Y;

			if (x > xmax)
				Location.X = xmax;
			else if (x < -xmax)
				Location.X = -xmax;
			
			if (y > ymax)
				Location.Y = ymax;
			else if (y < -ymax)
				Location.Y = -ymax;
		}
	}
}
