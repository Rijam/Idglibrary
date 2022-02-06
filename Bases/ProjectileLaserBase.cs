using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Bases
{
    public class ProjectileLaserBase : ModProjectile
    {

        public string laserTexture="Terraria/Projectile_" + ProjectileID.RocketII;
        public string laserTextureEnd="";
        public string laserTextureBeginning="";
        public float MoveDistance = 0f;
        public float MaxDistance = 2200f;
        public float CollisionDistance = 0f;
        public float Distance
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        public Vector2 hitspot
        {
            get { return projectile.Center+(projectile.velocity * Distance); }
        }

        public override string Texture
        {
            get { return "Terraria/Projectile_" + ProjectileID.RocketII; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("This is a base, you shouldn't see this!");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1f;
            projectile.timeLeft = 90;
            projectile.tileCollide = true;
            aiType = ProjectileID.WoodenArrowFriendly;
        }

        public virtual void MoreAI(Vector2 dustspot)
        {

        }

        public float movementFactor
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        // It appears that for this AI, only the ai0 field is used!
        public override void AI()
        {
            

            Vector2 start=projectile.Center;
            if (projectile.tileCollide)
            {
                for (Distance = MoveDistance; Distance <= MaxDistance; Distance += 5f)
                {
                    start = projectile.Center + projectile.velocity * Distance;
                    {
                        if ((!Collision.CanHit(projectile.Center, 1, 1, start, 1, 1)) && Distance > CollisionDistance)
                        {
                            Distance -= 5f;
                            break;
                        }
                    }
                }
            }
            else
            {
                Distance= MaxDistance;
            }

            projectile.position-=projectile.velocity;

            MoreAI(hitspot);


        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        Idglib.DrawTether(laserTexture,hitspot,projectile.Center,projectile.Opacity);
        return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //if (AtMaxCharge)
            //{
                Player player = Main.player[projectile.owner];
                Vector2 unit = projectile.velocity;
                float point = 0f;
                // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
                // It will look for collisions on the given line using AABB
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
                    hitspot, 22, ref point);
            //}
            //return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
        return false;
        }

    }
}