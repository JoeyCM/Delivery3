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

// Check request type
$requestType = $_GET['request'] ?? 'insert';

if ($requestType === 'insert') {
    // Handle data insertion
    $json = file_get_contents("php://input");

    if ($json === false || empty($json)) {
        echo "Error: No JSON input received.";
        exit;
    }

    $data = json_decode($json, true);

    if (!is_array($data)) {
        echo "Error: Invalid JSON input.";
        exit;
    }

    $eventType = $data['EventType'] ?? null;
    $timestamp = $data['Timestamp'] ?? null;
    $position = $data['Position'] ?? null;

    if ($eventType && $timestamp && $position) {
        $positionEscaped = $conn->real_escape_string($position);

        $sql = $conn->prepare("INSERT INTO events (EventType, Timestamp, Position) VALUES (?, ?, ?)");
        $sql->bind_param("sss", $eventType, $timestamp, $positionEscaped);

        if ($sql->execute()) {
            echo "Data inserted successfully.";
        } else {
            echo "Error: " . $sql->error;
        }
    } else {
        echo "Error: Missing required fields (EventType, Timestamp, Position).";
    }
} else if ($requestType === 'fetch') {
    // Fetch data for heatmap
    $eventType = $_GET['eventType'] ?? '';
    if (empty($eventType)) {
        echo "Error: Missing 'eventType' parameter.";
        exit;
    }

    $sql = "SELECT Position, COUNT(*) as Count FROM events WHERE EventType = ? GROUP BY Position";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("s", $eventType);
    $stmt->execute();

    $result = $stmt->get_result();
    $output = [];
    while ($row = $result->fetch_assoc()) {
        $output[] = $row;
    }

    echo json_encode($output);
} else {
    echo "Error: Invalid request type.";
}

$conn->close();
?>
