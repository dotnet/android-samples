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
using System.Collections.Generic;

// A particle system is just a collection of particles
namespace AccelerometerPlay
{
	class ParticleSystem
	{
		public long last_t;
		public float last_delta_t;

		static int NUM_PARTICLES = 15;
		public SimulationView sim_view;

		public List<Particle> Balls { get; private set; }

		public ParticleSystem (SimulationView view)
		{
			sim_view = view;
			Balls = new List<Particle> ();

			// Initially our particles have no speed or acceleration
			for (int i = 0; i < NUM_PARTICLES; i++)
				Balls.Add (new Particle (this));
		}

		// Update the position of each particle in the system using the
		// Verlet integrator.
		private void UpdatePositions (float sx, float sy, long timestamp)
		{
			long t = timestamp;

			if (last_t != 0) {
				float dT = (float)(t - last_t) * (1.0f / 1000000000.0f);

				if (last_delta_t != 0) {
					float dTC = dT / last_delta_t;

					foreach (var ball in Balls)
						ball.ComputePhysics (sx, sy, dT, dTC);
				}

				last_delta_t = dT;
			}

			last_t = t;
		}

		// Performs one iteration of the simulation. First updating the
		// position of all the particles and resolving the constraints and
		// collisions.
		public void Update (float sx, float sy, long now)
		{
			// update the system's positions
			UpdatePositions (sx, sy, now);

			// We do no more than a limited number of iterations
			int NUM_MAX_ITERATIONS = 10;
			
			// Resolve collisions, each particle is tested against every
			// other particle for collision. If a collision is detected the
			// particle is moved away using a virtual spring of infinite
			// stiffness.
			var random = new Random ();

			bool more = true;

			for (int k = 0; k < NUM_MAX_ITERATIONS && more; k++) {
				more = false;

				for (int i = 0; i < Balls.Count; i++) {
					var curr = Balls[i];

					for (int j = i + 1; j < Balls.Count; j++) {
						var ball = Balls[j];

						var dx = ball.Location.X - curr.Location.X;
						var dy = ball.Location.Y - curr.Location.Y;
						var dd = dx * dx + dy * dy;

						// Check for collisions
						if (dd <= SimulationView.BALL_DIAMETER_2) {
							
							// add a little bit of entropy, after nothing is
							// perfect in the universe.
							dx += ((float)random.Next () - 0.5f) * 0.0001f;
							dy += ((float)random.Next () - 0.5f) * 0.0001f;
							dd = dx * dx + dy * dy;

							// simulate the spring
							var d = (float)Math.Sqrt (dd);
							var c = (0.5f * (SimulationView.BALL_DIAMETER - d)) / d;

							curr.Location.X -= dx * c;
							curr.Location.Y -= dy * c;
							ball.Location.X += dx * c;
							ball.Location.Y += dy * c;

							more = true;
						}
					}

					// Finally make sure the particle doesn't intersects
					// with the walls.
					curr.ResolveCollisionWithBounds ();
				}
			}
		}
	}
}
