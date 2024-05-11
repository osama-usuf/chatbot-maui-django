from django.shortcuts import render

from rest_framework import viewsets
from rest_framework.response import Response
from .models import Item
from .serializers import ItemSerializer
from django.shortcuts import get_object_or_404

# View sets make things simpler, they simply define the view behavior.
# https://www.django-rest-framework.org/api-guide/viewsets/


class ItemViewSet(viewsets.ModelViewSet):
    # implements CRUD by default
    # https://www.django-rest-framework.org/api-guide/viewsets/#modelviewset
    queryset = Item.objects.all()
    serializer_class = ItemSerializer

# class ItemViewSet(viewsets.ViewSet):
#     queryset = Item.objects.all()
#     serializer_class = ItemSerializer


#     """
#     A simple ViewSet for listing or retrieving users.
#     """
#     def list(self, request):
#         queryset = Item.objects.all()
#         serializer = ItemSerializer(queryset)
#         return Response(serializer.data)

#     def retrieve(self, request, pk=None):
#         queryset = Item.objects.all()
#         item = get_object_or_404(queryset, pk=pk)
#         serializer = ItemSerializer(queryset)
#         return Response(serializer.data)