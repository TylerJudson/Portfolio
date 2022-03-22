

from typing import Tuple

import pygame
from pygame.locals import *
from SelectedStyle import SelectedStyle

from Surface import Surface
from Text import Text
from colors import DENIM


class TextBox:
    """Creates a textbox on the screen that is selectable and typeable
    """

    def __init__(self, pos: Tuple[int, int], surface: Surface, text: Text, fill: bool=True, fillColor: Tuple[int, int, int]=(0, 0, 0), border: bool=True, borderColor: Tuple[int, int, int]=(0, 0, 0), borderRadius: int=0, selectedStyle: SelectedStyle=None):
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

        self.fill = fill
        """Whether or not to fill the text box"""
        self.fillColor = fillColor
        """The color to fill the text box with"""

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

        self.isSelected = False
        """Whether the text box is selected or not"""

    def render(self):
        """Renders the textBox"""

        # clear the surface
        self.surface.clear()

        if (self.isSelected):
            # makes the border:
            if (self.border):
                pygame.draw.rect(self.selectedStyle.surface.display, self.selectedStyle.borderColor, self.rect, 2, self.borderRadius)

            # renders the text on the screen
            self.surface.display.blit(self.selectedStyle.text.display, self.selectedStyle.text.rect)
        else:
            # makes the border:
            if (self.border):
                pygame.draw.rect(self.surface.display, self.borderColor, self.rect, 2, self.borderRadius)

            pygame.draw.rect(self.surface.display, DENIM, self.rect, 0, self.borderRadius)

            # renders the text on the screen
            self.surface.display.blit(self.text.display, self.text.rect)

    def insert(self, index):
        """"""

    def backSpace(self):
        """"""

    def mouseClick(self, mousePos: Tuple[int, int]) -> bool:
        """Determines whether the mouse has clicked the button or not

        Args:
            mousePos (Tuple[int, int]): The positition of the mouse relative to the parent surface

        Returns:
            bool: Whether the mouse has clicked the button or not
        """

        # If the mouse pos is inside of the textbos
        if (self.pos[0] <= mousePos[0] <= self.pos[0] + self.surface.size[0] and self.pos[1] <= mousePos[1] <= self.pos[1] + self.surface.size[1]):
            self.isSelected = True
            return True

        self.isSelected = False
        return False
