<?php
// Database connection
$host = "localhost";
$username = "adriapm4";
$password = "54908860v";
$database = "adriapm4";

$conn = new mysqli($host, $username, $password, $database);
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Read JSON data
$json = file_get_contents("php://input");
$data = json_decode($json, true);

// Prepare SQL query
$eventType = $data['EventType'];
$timestamp = $data['Timestamp'];
$position = $conn->real_escape_string($data['Position']);

$sql = $conn->prepare("INSERT INTO events (EventType, Timestamp, Position) VALUES (?, ?, ?)");
$sql->bind_param("sss", $eventType, $timestamp, $position);

if ($sql->execute()) {
    echo "Data inserted successfully.";
} else {
    echo "Error: " . $sql->error;
}

$conn->close();
?>
