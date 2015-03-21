    

// Constructs the complex number x + yi.
function Complex(x, y) 
{
  this.x = x || 0; // Default to 0 if this parameter is undefined.
  this.y = y || 0;
}

// Returns a string representing this complex number in the form "x + yi".
Complex.prototype.toString = function() 
{
  return this.y >= 0 ? this.x + " + " + this.y + "i" : this.x + " - " + (-this.y) + "i";
}

// Returns a real number equal to the absolute value of this complex number.
Complex.prototype.modulus = function() 
{
  return Math.sqrt(this.x*this.x + this.y*this.y);
}

// Returns a complex number equal to the sum of the given complex number and this complex number.
Complex.prototype.add = function(z) 
{
  return new Complex(this.x + z.x, this.y + z.y);
}

// Returns a complex number equal to the square of this complex number.
Complex.prototype.square = function() 
{
  var x = this.x*this.x - this.y*this.y;
  var y = 2*this.x*this.y;
  
  return new Complex(x, y);
}