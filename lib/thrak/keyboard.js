

var keyboard = {};

keyboard._key = [];

keyboard._onKeyDown = function(event)
{
	console.log(event.keyCode);

	if(!keyboard._key[event.keyCode])
		keyboard._key[event.keyCode] = {};

	keyboard._key[event.keyCode].pressed = true;	
	if(keyboard._key[event.keyCode].onPress)
	{
		keyboard._key[event.keyCode].onPress(event.keyCode);
	}
}

keyboard._onKeyUp = function(event)
{
	if(!keyboard._key[event.keyCode])
		keyboard._key[event.keyCode] = {};

	keyboard._key[event.keyCode].pressed = false;	
	if(keyboard._key[event.keyCode].onRelease)
	{
		keyboard._key[event.keyCode].onRelease(event.keyCode);
	}	
}

keyboard.Initialize = function()
{
	document.onkeydown = keyboard._onKeyDown;
	document.onkeyup = keyboard._onKeyUp;	
}

keyboard.RegisterPressEvent = function(keyCode, callback)
{
	console.log(keyCode);

	if(!keyboard._key[keyCode])
		keyboard._key[keyCode] = {};
	
	keyboard._key[keyCode].onPress = callback;
}

keyboard.RegisterReleaseEvent = function(keyCode, callback)
{
	if(!keyboard._key[keyCode])
		keyboard._key[keyCode] = {};
	
	keyboard._key[keyCode].onRelease = callback;
}

keyboard.RegisterPressedEvent = function(keyCode, callback)
{
	if(!keyboard._key[keyCode])
		keyboard._key[keyCode] = {};
	
	keyboard._key[keyCode].whilePressed = callback;
}

keyboard._callPressedEvents = function(element, index, array)
{
	if(element.whilePressed)
	{
		element.whilePressed
	}
}

keyboard.Update = function()
{

}