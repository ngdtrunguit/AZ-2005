def sk_function(description=None, name=None):
    def decorator(func):
        func.description = description
        func.name = name
        return func
    return decorator

class CurrencyConverter:
    @sk_function(
        description="Convert an amount from one currency to another",
        name="convert"
    )
    def convert(self, amount: str, from_currency: str, to_currency: str) -> str:
        # Implement currency conversion logic here
        # This is a placeholder implementation
        return f"Converted {amount} from {from_currency} to {to_currency}"
