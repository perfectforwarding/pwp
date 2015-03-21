
var effectiveFPMS = 60 / 1000;

function Star(startingDistance, rotationSpeed)
{
  this.angle = 0;
  this.dist = startingDistance;
  this.rotSpeed = rotationSpeed;

  this.randomizeColors();
}

Star.prototype.Draw = function(tilt, spin, twinkle)
{
  // Set star position
  mat4.identity(mvMatrix);
  mat4.rotate(mvMatrix, deg2Rad(this.angle), [0, 1, 0]);
  mat4.translate(mvMatrix, [this.dist, 0, 0]);

  // Rotate back so that the star is facing the viewer
  mat4.rotate(mvMatrix, degToRad(-this.angle), [0.0, 1.0, 0.0]);
  mat4.rotate(mvMatrix, degToRad(-tilt), [1.0, 0.0, 0.0]);  

  // All stars spin around the Z axis at the same rate
  mat4.rotate(mvMatrix, degToRad(spin), [0.0, 0.0, 1.0]);  

  // Draw the star in its main color
  gl.uniform3f(shaderProgram.colorUniform, this.r, this.g, this.b);
  drawStar(); 
}

Star.prototype.Animate = function(elapsedTime)
{
  this.angle += this.rotSpeed * elapsedTime * effectiveFPMS;

  // Decrease the distance, resetting the star to the outside of
  // the spiral if it's at the center.
  this.dist -= 0.01 * effectiveFPMS * elapsedTime;
  if (this.dist < 0.0) {
      this.dist += 5.0;
      this.randomiseColors();
  }
}

Star.prototype.randomizeColors = function()
{
  // Give the star a random color for normal
  // circumstances...
  this.r = Math.random();
  this.g = Math.random();
  this.b = Math.random();  
}

