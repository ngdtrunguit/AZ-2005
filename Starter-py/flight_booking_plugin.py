def sk_function(description=None, name=None):
    def decorator(func):
        func.description = description
        func.name = name
        return func
    return decorator

class FlightBookingPlugin:
    @sk_function(
        description="Search for available flights",
        name="search_flights"
    )
    def search_flights(self, departure: str, destination: str, date: str) -> str:
        # Implement flight search logic here
        # This is a placeholder implementation
        return f"Found flights from {departure} to {destination} on {date}"
    
    @sk_function(
        description="Book a flight",
        name="book_flight"
    )
    def book_flight(self, flight_id: str) -> str:
        # Implement flight booking logic here
        # This is a placeholder implementation
        return f"Flight {flight_id} has been booked successfully"
