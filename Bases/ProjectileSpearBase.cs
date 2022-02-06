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
    public class ProjectileSpearBase : ModProjectile
    {
        public float movein=0.7f;
        public float moveout=0.6f;
        public float thrustspeed=3f;
        public float movementpercent=0.33f;

        public float truedirection=0f;
        public int projectileout=0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("This is a base, you shouldn't see this!");
        }

        public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Color drawColor)
        {
            bool facingleft=projectile.Center.X<Main.player[projectile.owner].Center.X;
            Microsoft.Xna.Framework.Graphics.SpriteEffects effect=SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(), drawColor, projectile.rotation+(facingleft ? (float)(1f * Math.PI) : 0f), origin, projectile.scale,facingleft ? effect : SpriteEffects.None, 0);
            return false;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.scale = 1.2f;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.hide = true;
        }

		//Make your spear shoot a custom projectile by overriding this in child classes, and writing your new projectile code here
		//use truedirection as a means to figure out which direction the spear was first shot
        public virtual void MakeProjectile()
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
            // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            // Sadly, Projectile/ModProjectile does not have its own
            Player projOwner = Main.player[projectile.owner];
            // Here we set some of the projectile's owner properties, such as held item and itemtime, along with projectile direction and position based on the player
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projectile.direction = projOwner.direction;
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            projectile.position.X = ownerMountedCenter.X - (float)(projectile.width / 2);
            projectile.position.Y = ownerMountedCenter.Y - (float)(projectile.height / 2);
            // As long as the player isn't frozen, the spear can move
            if (!projOwner.frozen)
            {
                if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
                {
                    movementFactor = thrustspeed; // Make sure the spear moves forward when initially thrown out
                    projectile.netUpdate = true; // Make sure to netUpdate this spear
                    truedirection = projectile.velocity.ToRotation();
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax * movementpercent) // Somewhere along the item animation, make sure the spear moves back
                {
                    if (projectileout<1){projectileout+=1;
                    truedirection = projectile.velocity.ToRotation();
                    MakeProjectile();
                    }
                    movementFactor -= movein;
                }
                else // Otherwise, increase the movement factor
                {
                    movementFactor += moveout;
                }
            }
            // Change the spear position based off of the velocity and the movementFactor
            projectile.position += projectile.velocity * movementFactor;
            // When we reach the end of the animation, we can kill the spear projectile
            if (projOwner.itemAnimation == 0)
            {
                projectile.Kill();
            }
            // Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
            // MathHelper.ToRadians(xx degrees here)
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            // Offset by 90 degrees here
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(90f);
            }
        }
    }
}