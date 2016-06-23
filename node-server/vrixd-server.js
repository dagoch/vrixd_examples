// A simple service to connect between a remote controller and a Unity app running on a mobile device
// The controller sends commands using REST Get requests
// The Unity app connects to the server via socket.io

// parse command line arguments
var argv = require('minimist')(process.argv.slice(2));

// default port
var httpPort = 4567;
// get port from -p command line argument, if given
if ("p" in argv) {
	console.log("got port parameter: "+argv.p);
	httpPort = argv.p;
}
var http = require('http');

var Router = require('node-simple-router');
var router = Router();

var server = http.createServer(router);


server.listen(httpPort, function(){
	console.log("Server listening on port "+httpPort);
})


var io = require('socket.io').listen(server);

// SOCKET CONNECTION
io.on('connection', function(socket){
	console.log('Unity connected')

	socket.on('disconnect', function(){
		console.log("Unity disconnected")
	})
})

// REST service receiving commands from the controller
router.get("/throw", function(request, response){
	io.emit("throw");
	console.log("Got message: throw")
	response.end("Got message: throw");
})

router.get("/move", function(request, response){
	io.emit("move");
	console.log("Got message: move")
	response.end("Got message: Move");
})

router.get("/move/:mag", function(request, response){
	io.emit("move",{ "mag": request.params.mag});
	console.log("Got message: move speed="+request.params.mag);
	response.end("Got message: Move "+request.params.mag);
})

router.get("/stop", function(request, response){
	io.emit("stop");
	console.log("Got message: stop")
	response.end("Got message: Stop");
})

router.get("/hello", function(request, response){
	console.log("Got message: hello")
	response.end("Got message: Hello.  Hello, Yun.");
})

// This resets the Unity game.
router.get("/restart", function(request, response){
	io.emit("restart");
	console.log("Got message: restart")
	response.end("Got message: restart");
})

router.get("/", function(request, response){
	response.end("THIS IS THE SERVER YOU'RE LOOKING FOR");
})



