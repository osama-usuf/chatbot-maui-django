from django.db import models

# https://docs.djangoproject.com/en/5.0/ref/models/fields/#django.db.models.DateField

# We need a model (defining an object)
# We need to serialize instances of an object to be transmitted over the API
# We need a view defining the query set using the serialized representation of given object
# And finally, we need a route/endpoint that defines the actual API

class Item(models.Model):
    name = models.CharField(max_length=50)
    details = models.TextField(max_length=100, null=True)
    date_added = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return self.name