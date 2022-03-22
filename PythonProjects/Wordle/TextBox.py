

from typing import Tuple

from pygame import Rect
from SelectedStyle import SelectedStyle

from Surface import Surface
from Text import Text


class TextBox:

    def __init__(self, pos: Tuple[int, int], surface: Surface, text: Text, border: bool=True, borderColor: Tuple[int, int, int]=(0, 0, 0), borderRadius: int=0, selectedStyle: SelectedStyle=None):
        """Initializes the TextBox

        Args:
            pos (Tuple[int, int]): The position of the Text Box on the screen
            surface (Surface): The surface of the Text Box used to display
            text (Text): The text of the Text Box
            border (bool, optional): Whether or not to display a border. Defaults to True.
            borderColor (Tuple[int, int, int], optional): The border color to display if border is True. Defaults to (0, 0, 0).
            borderRadius (int, optional): How rounded the corners of the border are. Defaults to 0.
            selectedStyle (SelectedStyle, optional): The Style the buton is when the Text Box is Selected. Defaults to None.
        """
        self.pos = pos
        """The position of the Text Box on the screen"""
        self.surface = surface
        """The surface of the Text Box used to display"""
        self.text = text
        """The text of the Text Box"""

        self.border = border
        """Whether or not to display a border"""
        self.borderColor = borderColor
        """The border color to display if border is True"""
        self.borderRadius = borderRadius
        """How rounded the corners of the border are"""

        self.selectedStyle = selectedStyle
        """The Style of the the Text Box when it is selected"""

        self.rect = Rect(self.surface.pos[0], self.surface.pos[0], self.surface.size[0], self.surface.size[1])
        """The Rect of the Text Box used for renderering"""

    def render(self):
        """"""

    def insert(self, index):
        """"""

    def backSpace(self):
        """"""
