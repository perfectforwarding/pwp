

/**
* Creates a new instance of a Camera, initializing it with the given arguments
*
* @param {vec3} vecLocation Location of the eye point
* @param {vec3} vecLookAt Location of the target look-at vector
* @param {vec3} vecUp Up-vector
* @param {number} vfov Vertical  FOV in degrees
* @param {number} aspect Ratio used to calculate horizontal FOV. Usually rendertarget with / height
* @param {number} near Distance to the near plane
* @param {number} far Distance to the far plane
*
* @returns {Camera} New Camera
*/

Camera = function(vecLocation, vecLookAt, vecUp, vfov, aspect, near, far)
{
	this.vfov = vfov;
	this.aspect = aspect;
	this.near = near;
	this.far = far;
	this.vecLocation = vecLocation;
	this.vecLookAt = vecLookAt;
	this.vecUp = vecUp;
}		    

Camera.prototype.setLocation = function(vecLocation)
{
	this.vecLocation = vecLocation;
}

Camera.prototype.setLookAt = function(vecLookAt)
{
	this.vecLookAt = vecLookAt;
}

Camera.prototype.getViewMatrix = function()
{
	return mat4.lookAt(this.vecLocation, this.vecLookAt, this.vecUp);
}

Camera.prototype.getProjectionMatrix = function()
{
	return mat4.perspective(this.vfov, this.aspect, this.near, this.far);
}

Camera.prototype.rotateEye = function(angle, vecAxis)
{
	vec3.subtract(this.vecLocation, this.vecLookAt);
	var qRot = quat4.fromAngleAxis(angle, vecAxis);
	quat4.multiplyVec3(qRot, this.vecLocation);
	vec3.add(this.vecLocation, this.vecLookAt);
}

Camera.prototype.rotateLookAt = function(angle, vecAxis)
{
	vec3.subtract(this.vecLookAt, this.vecLocation);
	var qRot = quat4.fromAngleAxis(angle, vecAxis);
	quat4.multiplyVec3(qRot, this.vecLookAt);
	vec3.add(this.vecLookAt, this.vecLocation);
}

Camera.prototype.getEyeDir = function()
{
	var dir = vec3.create();
	vec3.subtract(this.vecLookAt, this.vecLocation, dir);
	vec3.normalize(dir);
	return dir;
}

Camera.prototype.getForwardAxis = function()
{
	var mtx = this.getViewMatrix();
	return vec3.createFrom(-mtx[2], -mtx[6], -mtx[10]);	
}

Camera.prototype.getRightAxis = function()
{
	var mtx = this.getViewMatrix();
	return vec3.createFrom(mtx[0], mtx[4], mtx[8]);	
}

Camera.prototype.getUpAxis = function()
{
	var mtx = this.getViewMatrix();
	return vec3.createFrom(mtx[1], mtx[5], mtx[9]);	
}