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

        public string laserTexture="Terraria/Images/Projectile_" + ProjectileID.RocketII;
        public string laserTextureEnd="";
        public string laserTextureBeginning="";
        public float MoveDistance = 0f;
        public float MaxDistance = 2200f;
        public float CollisionDistance = 0f;
        public float Distance
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public Vector2 hitspot
        {
            get { return Projectile.Center+(Projectile.velocity * Distance); }
        }

        public override string Texture
        {
            get { return "Terraria/Images/Projectile_" + ProjectileID.RocketII; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("This is a base, you shouldn't see this!");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = true;
            AIType = ProjectileID.WoodenArrowFriendly;
        }

        public virtual void MoreAI(Vector2 dustspot)
        {

        }

        public float movementFactor
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        // It appears that for this AI, only the ai0 field is used!
        public override void AI()
        {
            

            Vector2 start=Projectile.Center;
            if (Projectile.tileCollide)
            {
                for (Distance = MoveDistance; Distance <= MaxDistance; Distance += 5f)
                {
                    start = Projectile.Center + Projectile.velocity * Distance;
                    {
                        if ((!Collision.CanHit(Projectile.Center, 1, 1, start, 1, 1)) && Distance > CollisionDistance)
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

            Projectile.position-=Projectile.velocity;

            MoreAI(hitspot);


        }

        public override bool PreDraw(ref Color lightColor)
        {
        Idglib.DrawTether(laserTexture,hitspot,Projectile.Center,Projectile.Opacity);
        return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //if (AtMaxCharge)
            //{
                Player player = Main.player[Projectile.owner];
                Vector2 unit = Projectile.velocity;
                float point = 0f;
                // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
                // It will look for collisions on the given line using AABB
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
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