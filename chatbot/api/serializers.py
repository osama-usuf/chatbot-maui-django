# Take a model and convert to a serialized JSON object

# See example from https://www.django-rest-framework.org/#installation
from rest_framework import serializers
from .models import Item

# Serializers define the API representation.
class ItemSerializer(serializers.HyperlinkedModelSerializer):
    class Meta:
        model = Item
        fields = ['pk', 'name', 'details']