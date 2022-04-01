

from copy import deepcopy
from pickle import TRUE
from typing import Tuple

import pygame
from pygame.locals import *
from Cursor import Cursor
from Style import Style
from Surface import Surface


class TextBox:
    """Creates a textbox on the screen that is selectable and typeable
    """

    def __init__(self, surface: Surface, style: Style, selectedStyle: Style, hidden: bool=False):
        """Initializes the TextBox

        Args:
            surface (Surface): The surface of the Text Box used to display
            style (Style): The style of the text box used to display
            selectedStyle (Style): The Style the text box is when the Text Box is Selected. Defaults to None.
            hidden (bool): Whether the text should be hidden or not
        """
        self.surface = surface
        """The surface of the Text Box used to display"""
        
        self.style = style
        """The style to display"""

        self.selectedStyle = selectedStyle
        """The Style of the the Text Box when it is selected"""

        self.hidden = hidden
        """Whether the text is hidden or not"""

        self.rect = Rect(0, 0, self.surface.size[0], self.surface.size[1])
        """The Rect of the Text Box used for renderering"""

        self.isSelected = False
        """Whether the text box is selected or not"""

        self.cursor = Cursor(self.style.text, self.style.text.color)
        """The cursor that blinks"""
        
    def render(self):
        """Renders the textBox"""

        # clear the surface
        self.surface.clear()

        if (self.isSelected):
            # fills the text box:
            if (self.selectedStyle.fillColor != None):
                pygame.draw.rect(self.surface.display, self.selectedStyle.fillColor,
                                 self.rect, 0, self.selectedStyle.borderRadius)

            # makes the border:
            if (self.selectedStyle.borderColor != None):
                pygame.draw.rect(self.surface.display, self.selectedStyle.borderColor, 
                                    self.rect, self.selectedStyle.borderWidth, self.selectedStyle.borderRadius)
            if (self.style.text != None):
                text = self.style.text.text
                if (self.hidden):
                    self.style.text.text = "•" * len(text)
                # renders the text on the screen
                self.surface.display.blit(self.style.text.display, self.style.text.rect)
                self.style.text.text = text

            # render the cursor
            self.cursor.render(self.surface.display)
        else:
            # fills the text box:
            if (self.style.fillColor != None):
                pygame.draw.rect(self.surface.display, self.style.fillColor,
                                 self.rect, 0, self.style.borderRadius)

            # makes the border:
            if (self.style.borderColor != None):
                pygame.draw.rect(self.surface.display, self.style.borderColor,
                 self.rect, self.style.borderWidth, self.style.borderRadius)

            if (self.style.text != None):
                text = self.style.text.text
                if (self.hidden):
                    self.style.text.text = "•" * len(text)
                # renders the text on the screen
                self.surface.display.blit(self.style.text.display, self.style.text.rect)
                self.style.text.text = text

    def insert(self, char: str):
        """Inserts a character into the text box

        Args:
            char (str): The character to insert
        """
        self.style.text.text += char

        text = self.style.text.text
        if (self.hidden):
            self.style.text.text = "•" * len(text)
        # renders the text on the screen
        self.cursor.move(self.style.text)
        self.style.text.text = text
            

    def backSpace(self):
        """Deletes the character the cursor is on in the text box"""
        self.style.text.text = self.style.text.text[:-1]
        
        text = self.style.text.text
        if (self.hidden):
            self.style.text.text = "•" * len(text)
        # renders the text on the screen
        self.cursor.move(self.style.text)
        self.style.text.text = text


    def mouseClick(self, mousePos: Tuple[int, int]) -> bool:
        """Determines whether the mouse has clicked the button or not

        Args:
            mousePos (Tuple[int, int]): The positition of the mouse relative to the parent surface

        Returns:
            bool: Whether the mouse has clicked the button or not
        """

        # If the mouse pos is inside of the textbos
        if (self.surface.pos[0] <= mousePos[0] <= self.surface.pos[0] + self.surface.size[0] and self.surface.pos[1] <= mousePos[1] <= self.surface.pos[1] + self.surface.size[1]):
            self.isSelected = True
            return True

        self.isSelected = False
        return False
