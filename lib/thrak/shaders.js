/**
 * Creates and compiles a shader.
 *
 * @param {!WebGLRenderingContext} gl The WebGL Context.
 * @param {string} shaderSource The GLSL source code for the shader.
 * @param {number} shaderType The type of shader, VERTEX_SHADER or
 *     FRAGMENT_SHADER.
 * @return {!WebGLShader} The shader.
 */
function compileShader(gl, shaderSource, shaderType) 
{
  // Create the shader object
  var shader = gl.createShader(shaderType);

  // Set the shader source code.
  gl.shaderSource(shader, shaderSource);

  // Compile the shader
  gl.compileShader(shader);

  // Check if it compiled
  var success = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
  if (!success) {
    // Something went wrong during compilation; get the error
    console.log(gl.getShaderInfoLog(shader));
    throw "could not compile shader:" + gl.getShaderInfoLog(shader);
  }

  return shader;
}

function getShaderSource(id)
{
    var shaderScript = document.getElementById(id);
    if(shaderScript)
    {
        var str = "";
        var k = shaderScript.firstChild;
        while (k) 
        {
            if (k.nodeType == 3) 
            {
                str += k.textContent;
            }
            k = k.nextSibling;
        }

        return str;
    }
}

function getShader(gl, id) 
{
    var shader = null;

    var shaderScript = document.getElementById(id);
    if (shaderScript) 
    {
        var str = "";
        var k = shaderScript.firstChild;
        while (k) 
        {
            if (k.nodeType == 3) 
            {
                str += k.textContent;
            }
            k = k.nextSibling;
        }

        if (shaderScript.type == "x-shader/x-fragment") 
        {
        	shader = compileShader(gl, str, gl.FRAGMENT_SHADER);
        } 
        else if (shaderScript.type == "x-shader/x-vertex") 
        {
        	shader = compileShader(gl, str, gl.VERTEX_SHADER);
        }
    }

    return shader;
}

function linkShaderProgram(gl_ctx, vertexShader, fragmentShader)
{
    shaderProgram = gl_ctx.createProgram();

    gl_ctx.attachShader(shaderProgram, vertexShader);
    gl_ctx.attachShader(shaderProgram, fragmentShader);
    gl_ctx.linkProgram(shaderProgram);
    if (!gl_ctx.getProgramParameter(shaderProgram, gl_ctx.LINK_STATUS)) 
    {
        alert("Could not initialise shaders");
    }

    return shaderProgram;                   
}

/**
 * Creates a program from 2 shaders.
 *
 * @param {!WebGLRenderingContext) gl The WebGL context.
 * @param {!WebGLShader} vertexShader A vertex shader.
 * @param {!WebGLShader} fragmentShader A fragment shader.
 * @return {!WebGLProgram} A program.
 */
function createProgram(gl, vertexShader, fragmentShader) 
{
  // create a program.
  var program = gl.createProgram();

  // attach the shaders.
  gl.attachShader(program, vertexShader);
  gl.attachShader(program, fragmentShader);

  // link the program.
  gl.linkProgram(program);

  // Check if it linked.
  var success = gl.getProgramParameter(program, gl.LINK_STATUS);
  if (!success) {
      // something went wrong with the link
      throw ("program filed to link:" + gl.getProgramInfoLog (program));
  }

  return program;
}

function loadShaderProgram(gl, vertex_shader_url, fragment_shader_url, successCallback, errorCallback)
{
    // Check to see if the counter has been initialized
    if ( typeof loadShaderProgram.remainingPrograms == 'undefined' ) {
        // It has not... perform the initialization
        loadShaderProgram.remainingPrograms = 1;
    }
    else {
    	++loadShaderProgram.remainingPrograms;
    }

	// Init copy shader
	loadFiles(
		[vertex_shader_url, fragment_shader_url], 
		function (shaderText) {
			var shaderProgram = linkShaderProgram(
				gl, 
				compileShader(gl, shaderText[0], gl.VERTEX_SHADER), 
				compileShader(gl, shaderText[1], gl.FRAGMENT_SHADER));

			if(shaderProgram) {
				--loadShaderProgram.remainingPrograms;
				successCallback(shaderProgram);
			} else {
				errorCallback('Failed to compile');
			}
		},
		errorCallback
	); 	
}